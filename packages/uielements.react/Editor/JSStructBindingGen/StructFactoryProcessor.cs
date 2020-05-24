using System;
using System.Collections.Generic;
using ChakraHost.Hosting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.ReactUIElements.JsStructBinding.CodeGen.Fields;
using UnityReactUIElements;

using MethodAttributes = Mono.Cecil.MethodAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen
{
    internal static class StructFactoryProcessor
    {
        public static void Process(TypeReference type, AssemblyDefinition definition, IEnumerable<TypeReference> supportedTypes)
        {
            definition.MainModule.Types.Add(CreateJsTypeFactoryDefinition(type, definition.MainModule, supportedTypes));
        }

        private static TypeDefinition CreateJsTypeFactoryDefinition(
            TypeReference type,
            ModuleDefinition mainModule,
            IEnumerable<TypeReference> supportedTypes)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));
            var objectTypeDefinition = mainModule.TypeSystem.Object;
            var iJSBindingFactoryDefinition = mainModule.ImportReference(typeof(IJSBindingFactory));

            var definition = new TypeDefinition(type.Namespace, $"{type.Name}_JsFactory", TypeAttributes.Class, objectTypeDefinition);
            definition.Interfaces.Add(new InterfaceImplementation(iJSBindingFactoryDefinition));

            AddEmptyConstructor(definition, mainModule, mainModule.ImportReference(typeof(object).GetConstructor(Type.EmptyTypes)));

            var prototypeField = new FieldDefinition("prototype", FieldAttributes.Private, javascriptValueTypeDefinition);
            var propertyAccessors = new Dictionary<string, (MethodDefinition getter, MethodDefinition setter)>();

            foreach (var field in type.Resolve().Fields)
            {
                var reference = field.Module != mainModule
                    ? mainModule.ImportReference(field)
                    : field;

                propertyAccessors.Add(field.Name, (
                    FieldProcessor.CreateGetter(type, reference, mainModule, supportedTypes),
                    FieldProcessor.CreateSetter(type, reference, mainModule, supportedTypes)
                ));

                definition.Methods.Add(propertyAccessors[reference.Name].getter);
                definition.Methods.Add(propertyAccessors[reference.Name].setter);
            }

            var finalizerMethod = CreateFinalizerMethod(mainModule);
            var createJsObjectForNativeMethod = CreateJsObjectForNative(mainModule, prototypeField,
                finalizerMethod, propertyAccessors);
            var constructorMethod = ConstructorCallbackMethod(type, createJsObjectForNativeMethod, mainModule);
            var getNameMethod = CreateNamePropertyGetMethod(type, mainModule);

            definition.Fields.Add(prototypeField);
            definition.Methods.Add(createJsObjectForNativeMethod);
            definition.Methods.Add(getNameMethod);
            definition.Methods.Add(constructorMethod);
            definition.Methods.Add(finalizerMethod);
            definition.Methods.Add(AddCreateConstructorMethod(constructorMethod, mainModule));

            var nameProperty = new PropertyDefinition("Name", PropertyAttributes.None, mainModule.TypeSystem.String);

            nameProperty.GetMethod = getNameMethod;

            definition.Properties.Add(nameProperty);

            return definition;
        }

        private static MethodDefinition CreateJsObjectForNative(
            ModuleDefinition mainModule,
            FieldDefinition prototype,
            MethodDefinition finalizerMethod,
            Dictionary<string, (MethodDefinition getter, MethodDefinition setter)> fields)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));
            var isValid = mainModule.ImportReference(typeof(JavaScriptValue).GetMethod("get_IsValid"));

            var method = new MethodDefinition("CreateJsObjectForNative", MethodAttributes.Public | MethodAttributes.Virtual, javascriptValueTypeDefinition);

            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.Void.MakePointerType()));

            var isValidResultVariable = new VariableDefinition(mainModule.TypeSystem.Boolean);
            var javascriptValue = new VariableDefinition(mainModule.ImportReference(typeof(JavaScriptValue)));

            method.Body.Variables.Add(isValidResultVariable);
            method.Body.Variables.Add(javascriptValue);

            var javaScriptNativeFunctionCtor =
                mainModule.ImportReference(typeof(JavaScriptNativeFunction).GetConstructor(new[] { typeof(object), typeof(IntPtr) }));

            var javaScriptPropertyFromStringMethod =
                mainModule.ImportReference(typeof(JavaScriptPropertyId).GetMethod(nameof(JavaScriptPropertyId.FromString)));

            var setPropertyMethod =
                mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.SetProperty)));

            var createFunctionMethod = mainModule.ImportReference(typeof(JavaScriptValue).GetMethod("CreateFunction", new[] { typeof(JavaScriptNativeFunction) }));


            var ilProcessor = method.Body.GetILProcessor();

            var firstInstructionAfterCondition = Instruction.Create(OpCodes.Ldarg_1);

            // if (!prototype.IsValid)
            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Ldflda, prototype);
            ilProcessor.Emit(OpCodes.Call, isValid);
            ilProcessor.Emit(OpCodes.Ldc_I4_0);
            ilProcessor.Emit(OpCodes.Ceq);
            ilProcessor.Emit(OpCodes.Stloc_S, isValidResultVariable);
            ilProcessor.Emit(OpCodes.Ldloc_S, isValidResultVariable);
            ilProcessor.Emit(OpCodes.Brfalse, firstInstructionAfterCondition);
            ilProcessor.Emit(OpCodes.Nop);

            // var prototype = JavaScriptValue.CreateObject();
            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.CreateObject))));
            ilProcessor.Emit(OpCodes.Stfld, prototype);

            foreach (var field in fields.Keys)
            {
                var getterVariable = new VariableDefinition(javascriptValueTypeDefinition);
                method.Body.Variables.Add(getterVariable);

                var setterVariable = new VariableDefinition(javascriptValueTypeDefinition);
                method.Body.Variables.Add(setterVariable);

                // var get[field]Function = JavaScriptValue.CreateFunction(Get_[field]);
                ilProcessor.Emit(OpCodes.Ldarg_0);
                ilProcessor.Emit(OpCodes.Ldftn, fields[field].getter);
                ilProcessor.Emit(OpCodes.Newobj, javaScriptNativeFunctionCtor);
                ilProcessor.Emit(OpCodes.Call, createFunctionMethod);
                ilProcessor.Emit(OpCodes.Stloc_S, getterVariable);

                // var set[field]Function = JavaScriptValue.CreateFunction(Set_[field]);
                ilProcessor.Emit(OpCodes.Ldarg_0);
                ilProcessor.Emit(OpCodes.Ldftn, fields[field].setter);
                ilProcessor.Emit(OpCodes.Newobj, javaScriptNativeFunctionCtor);
                ilProcessor.Emit(OpCodes.Call, createFunctionMethod);
                ilProcessor.Emit(OpCodes.Stloc_S, setterVariable);

                // prototype.SetProperty(JavaScriptPropertyId.FromString("get_count"), getCountFunction, false);
                ilProcessor.Emit(OpCodes.Ldarg_0);
                ilProcessor.Emit(OpCodes.Ldflda, prototype);
                ilProcessor.Emit(OpCodes.Ldstr, $"get{field}");
                ilProcessor.Emit(OpCodes.Call, javaScriptPropertyFromStringMethod);
                ilProcessor.Emit(OpCodes.Ldloc_S, getterVariable);
                ilProcessor.Emit(OpCodes.Ldc_I4_0);
                ilProcessor.Emit(OpCodes.Call, setPropertyMethod);
                ilProcessor.Emit(OpCodes.Nop);

                // prototype.SetProperty(JavaScriptPropertyId.FromString("set_count"), setCountFunction, false);
                ilProcessor.Emit(OpCodes.Ldarg_0);
                ilProcessor.Emit(OpCodes.Ldflda, prototype);
                ilProcessor.Emit(OpCodes.Ldstr, $"set{field}");
                ilProcessor.Emit(OpCodes.Call, javaScriptPropertyFromStringMethod);
                ilProcessor.Emit(OpCodes.Ldloc_S, setterVariable);
                ilProcessor.Emit(OpCodes.Ldc_I4_0);
                ilProcessor.Emit(OpCodes.Call, setPropertyMethod);
                ilProcessor.Emit(OpCodes.Nop);
            }

            // var javaScriptValue = JavaScriptValue.CreateExternalObject((IntPtr)ptr, Finalizer);
            ilProcessor.Append(firstInstructionAfterCondition);
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(IntPtr).GetMethod("op_Explicit", new[] { typeof(void*) })));
            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Ldftn, finalizerMethod);
            ilProcessor.Emit(OpCodes.Newobj, mainModule.ImportReference(typeof(Action<IntPtr>).GetConstructor(new[] { typeof(object), typeof(IntPtr) })));
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.CreateExternalObject))));
            ilProcessor.Emit(OpCodes.Stloc_S, javascriptValue);

            // javaScriptValue.Prototype = prototype;
            ilProcessor.Emit(OpCodes.Ldloca_S, javascriptValue);
            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Ldfld, prototype);
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(JavaScriptValue).GetMethod("set_Prototype")));
            ilProcessor.Emit(OpCodes.Nop);

            // return javaScriptValue;
            ilProcessor.Emit(OpCodes.Ldloc_S, javascriptValue);
            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodDefinition CreateNamePropertyGetMethod(TypeReference type, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition(
                "get_Name",
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.NewSlot,
                mainModule.TypeSystem.String);

            var ilProcessor = method.Body.GetILProcessor();
            ilProcessor.Emit(OpCodes.Ldstr, type.Name);
            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }

        private static void AddEmptyConstructor(TypeDefinition type, ModuleDefinition module, MethodReference baseEmptyConstructor)
        {
            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var method = new MethodDefinition(".ctor", methodAttributes, module.TypeSystem.Void);
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, baseEmptyConstructor));
            method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
            type.Methods.Add(method);
        }

        private static MethodDefinition AddCreateConstructorMethod(MethodDefinition constructorMethod, ModuleDefinition mainModule)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));

            var method = new MethodDefinition("CreateConstructor", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final,
                javascriptValueTypeDefinition);

            var ilProcessor = method.Body.GetILProcessor();

            var javaScriptNativeFunctionCtor =
                mainModule.ImportReference(typeof(JavaScriptNativeFunction).GetConstructor(new [] { typeof(object), typeof(IntPtr) }));

            var createFunctionMethod = mainModule.ImportReference(typeof(JavaScriptValue).GetMethod("CreateFunction", new[] { typeof(JavaScriptNativeFunction) }));

            // return JavaScriptValue.CreateFunction(Constructor);
            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Ldftn, constructorMethod);
            ilProcessor.Emit(OpCodes.Newobj, javaScriptNativeFunctionCtor);
            ilProcessor.Emit(OpCodes.Call, createFunctionMethod);
            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodDefinition ConstructorCallbackMethod(
            TypeReference type,
            MethodDefinition createJsObjectForNativeMethod,
            ModuleDefinition mainModule)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));

            var method = new MethodDefinition("Constructor", MethodAttributes.Private,
                javascriptValueTypeDefinition);

            method.Parameters.Add(new ParameterDefinition(javascriptValueTypeDefinition));
            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.Boolean));
            method.Parameters.Add(new ParameterDefinition(mainModule.ImportReference(typeof(JavaScriptValue[]))));
            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.Int16));
            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.IntPtr));

            var obj = new VariableDefinition(type);
            var size = new VariableDefinition(mainModule.TypeSystem.Int32);
            var alignment = new VariableDefinition(mainModule.TypeSystem.Int32);
            var ptr = new VariableDefinition(mainModule.TypeSystem.Void.MakePointerType());

            method.Body.Variables.Add(obj);
            method.Body.Variables.Add(size);
            method.Body.Variables.Add(alignment);
            method.Body.Variables.Add(ptr);

            var sizeOfMethod = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.SizeOf), new Type[] { }))
                .SetGenericParameter(type);

            var alignOfMethod = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.AlignOf), new Type[] { }))
                .SetGenericParameter(type);

            var copyStructureToPtrMethod = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.CopyStructureToPtr)))
                .SetGenericParameter(type);

            var ilProcessor = method.Body.GetILProcessor();


            // var obj = new CounterComponent();
            ilProcessor.Emit(OpCodes.Ldloca_S, obj);
            ilProcessor.Emit(OpCodes.Initobj, type);

            // var size = UnsafeUtility.SizeOf<CounterComponent>();
            ilProcessor.Emit(OpCodes.Call, sizeOfMethod);
            ilProcessor.Emit(OpCodes.Stloc_S, size);

            // var alignment = UnsafeUtility.AlignOf<CounterComponent>();
            ilProcessor.Emit(OpCodes.Call, alignOfMethod);
            ilProcessor.Emit(OpCodes.Stloc_S, alignment);
            ilProcessor.Emit(OpCodes.Nop);

            // var ptr = UnsafeUtility.Malloc(size, alignment, Allocator.TempJob);
            ilProcessor.Emit(OpCodes.Ldloc_S, size);
            ilProcessor.Emit(OpCodes.Conv_I8);
            ilProcessor.Emit(OpCodes.Ldloc_S, alignment);
            ilProcessor.Emit(OpCodes.Ldc_I4_4);
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.Malloc), new[] { typeof(long), typeof(int), typeof(Allocator) })));
            ilProcessor.Emit(OpCodes.Stloc_S, ptr);

            // UnsafeUtility.CopyStructureToPtr(ref obj, ptr);
            ilProcessor.Emit(OpCodes.Ldloca_S, obj);
            ilProcessor.Emit(OpCodes.Ldloc_S, ptr);
            ilProcessor.Emit(OpCodes.Call, copyStructureToPtrMethod);
            ilProcessor.Emit(OpCodes.Nop);

            // return javaScriptValue;
            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Ldloc_S, ptr);
            ilProcessor.Emit(OpCodes.Call, createJsObjectForNativeMethod);
            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodDefinition CreateFinalizerMethod(ModuleDefinition mainModule)
        {
            var voidTypeDefinition = mainModule.TypeSystem.Void;

            var method = new MethodDefinition("Finalizer", MethodAttributes.Private, voidTypeDefinition);

            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.IntPtr));

            var ilProcessor = method.Body.GetILProcessor();

            // UnsafeUtility.Free((void*)data, Allocator.Persistent);
            ilProcessor.Emit(OpCodes.Ldarg_1);
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(IntPtr).GetMethod("op_Explicit", new[] { typeof(void*) })));
            ilProcessor.Emit(OpCodes.Ldc_I4_4);
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.Free), new[] { typeof(void*), typeof(Allocator) })));

            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }
    }
}