using System;
using System.Collections.Generic;
using ChakraHost.Hosting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
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
            var objectTypeDefinition = mainModule.TypeSystem.Object;
            var iJSBindingFactoryDefinition = mainModule.ImportReference(typeof(IJSBindingFactory));

            var definition = new TypeDefinition(type.Namespace, $"{type.Name}_JsFactory", TypeAttributes.Class, objectTypeDefinition);
            definition.Interfaces.Add(new InterfaceImplementation(iJSBindingFactoryDefinition));

            TypeUtils.AddEmptyConstructor(definition, mainModule, mainModule.ImportReference(typeof(object).GetConstructor(Type.EmptyTypes)));

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
            var noopFinalizerMethod = CreateNoopFinalizerMethod(mainModule);

            var createJsObjectForNativeMethod = CreateJsObjectForNative(
                mainModule,
                finalizerMethod,
                noopFinalizerMethod,
                propertyAccessors);

            var constructorMethod = ConstructorCallbackMethod(type, createJsObjectForNativeMethod, mainModule);
            var getNameMethod = CreateNamePropertyGetMethod(type, mainModule);
            var getComponentSizeMethod = CreateComponentSizePropertyGetMethod(type, mainModule);
            var entityCommandBufferSetComponentMethod = CreateEntityCommandBufferSetComponentMethod(type, mainModule);
            var getReadComponentTypeMethod = CreateReadComponentTypePropertyGetMethod(type, mainModule);

            definition.Methods.Add(entityCommandBufferSetComponentMethod);
            definition.Methods.Add(createJsObjectForNativeMethod);
            definition.Methods.Add(getNameMethod);
            definition.Methods.Add(getComponentSizeMethod);
            definition.Methods.Add(getReadComponentTypeMethod);
            definition.Methods.Add(constructorMethod);
            definition.Methods.Add(finalizerMethod);
            definition.Methods.Add(noopFinalizerMethod);
            definition.Methods.Add(AddCreateConstructorMethod(constructorMethod, mainModule));

            var nameProperty = new PropertyDefinition("Name", PropertyAttributes.None, mainModule.TypeSystem.String);
            nameProperty.GetMethod = getNameMethod;

            var componentSizeProperty = new PropertyDefinition("ComponentSize", PropertyAttributes.None, mainModule.TypeSystem.Int32);
            componentSizeProperty.GetMethod = getComponentSizeMethod;

            var readComponentProperty = new PropertyDefinition("ReadComponentType", PropertyAttributes.None, mainModule.ImportReference(typeof(ComponentType)));
            readComponentProperty.GetMethod = getReadComponentTypeMethod;

            definition.Properties.Add(nameProperty);
            definition.Properties.Add(componentSizeProperty);
            definition.Properties.Add(readComponentProperty);

            return definition;
        }

        private static MethodDefinition CreateEntityCommandBufferSetComponentMethod(TypeReference type, ModuleDefinition mainModule)
        {
            //EntityCommandBuffer buffer, Entity e, void* componentPtr
            var entityCommandBufferType = mainModule.ImportReference(typeof(EntityCommandBuffer));
            var entityType = mainModule.ImportReference(typeof(Entity));
            var voidPtrType = mainModule.TypeSystem.Void.MakePointerType();
            var setComponentMethod = mainModule.ImportReference(typeof(EntityCommandBuffer)
                .GetMethod("SetComponent")).SetGenericParameter(type);
            var copyPtrToStructureMethod = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.CopyPtrToStructure)))
                .SetGenericParameter(type);

            var method = new MethodDefinition("EntityCommandBufferSetComponent", MethodAttributes.Public | MethodAttributes.Virtual, mainModule.TypeSystem.Void);
            method.Parameters.Add(new ParameterDefinition(entityCommandBufferType));
            method.Parameters.Add(new ParameterDefinition(entityType));
            method.Parameters.Add(new ParameterDefinition(voidPtrType));

            var structVariable = new VariableDefinition(type);
            method.Body.Variables.Add(structVariable);

            var ilProcessor = method.Body.GetILProcessor();

            ilProcessor.Emit(OpCodes.Ldarg_S, method.Parameters[2]);
            ilProcessor.Emit(OpCodes.Ldloca_S, structVariable);
            ilProcessor.Emit(OpCodes.Call, copyPtrToStructureMethod);

            ilProcessor.Emit(OpCodes.Ldarga_S, method.Parameters[0]);
            ilProcessor.Emit(OpCodes.Ldarg_S, method.Parameters[1]);
            ilProcessor.Emit(OpCodes.Ldloc_S, structVariable);
            ilProcessor.Emit(OpCodes.Call, setComponentMethod);
            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodDefinition CreateJsObjectForNative(
            ModuleDefinition mainModule,
            MethodDefinition finalizerMethod,
            MethodDefinition noopFinalizerMethod,
            Dictionary<string, (MethodDefinition getter, MethodDefinition setter)> fields)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));

            var method = new MethodDefinition("CreateJsObjectForNative", MethodAttributes.Public | MethodAttributes.Virtual, javascriptValueTypeDefinition);

            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.Void.MakePointerType()));
            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.Boolean));

            var isValidResultVariable = new VariableDefinition(mainModule.TypeSystem.Boolean);
            var javascriptValue = new VariableDefinition(mainModule.ImportReference(typeof(JavaScriptValue)));
            var prototypeVariable = new VariableDefinition(mainModule.ImportReference(typeof(JavaScriptValue)));

            method.Body.Variables.Add(isValidResultVariable);
            method.Body.Variables.Add(javascriptValue);
            method.Body.Variables.Add(prototypeVariable);

            var javaScriptNativeFunctionCtor =
                mainModule.ImportReference(typeof(JavaScriptNativeFunction).GetConstructor(new[] { typeof(object), typeof(IntPtr) }));

            var javaScriptPropertyFromStringMethod =
                mainModule.ImportReference(typeof(JavaScriptPropertyId).GetMethod(nameof(JavaScriptPropertyId.FromString)));

            var setPropertyMethod =
                mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.SetProperty)));

            var createFunctionMethod = mainModule.ImportReference(typeof(JavaScriptValue).GetMethod("CreateFunction", new[] { typeof(string), typeof(JavaScriptNativeFunction) }));


            var ilProcessor = method.Body.GetILProcessor();

            var firstInstructionAfterCondition = Instruction.Create(OpCodes.Ldarg_2);

            // var prototype = JavaScriptValue.CreateObject();
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.CreateObject))));
            ilProcessor.Emit(OpCodes.Stloc_S, prototypeVariable);

            foreach (var field in fields.Keys)
            {
                var getterVariable = new VariableDefinition(javascriptValueTypeDefinition);
                method.Body.Variables.Add(getterVariable);

                var setterVariable = new VariableDefinition(javascriptValueTypeDefinition);
                method.Body.Variables.Add(setterVariable);

                var propertyDescriptor = new VariableDefinition(javascriptValueTypeDefinition);
                method.Body.Variables.Add(propertyDescriptor);

                // var get[field]Function = JavaScriptValue.CreateFunction(Get_[field]);
                ilProcessor.Emit(OpCodes.Ldstr, "Get_Field");
                ilProcessor.Emit(OpCodes.Ldnull);
                ilProcessor.Emit(OpCodes.Ldftn, fields[field].getter);
                ilProcessor.Emit(OpCodes.Newobj, javaScriptNativeFunctionCtor);
                ilProcessor.Emit(OpCodes.Call, createFunctionMethod);
                ilProcessor.Emit(OpCodes.Stloc_S, getterVariable);

                // var set[field]Function = JavaScriptValue.CreateFunction(Set_[field]);
                ilProcessor.Emit(OpCodes.Ldstr, "Set_Field");
                ilProcessor.Emit(OpCodes.Ldnull);
                ilProcessor.Emit(OpCodes.Ldftn, fields[field].setter);
                ilProcessor.Emit(OpCodes.Newobj, javaScriptNativeFunctionCtor);
                ilProcessor.Emit(OpCodes.Call, createFunctionMethod);
                ilProcessor.Emit(OpCodes.Stloc_S, setterVariable);

                //var [field]PropertyDescriptor = JavaScriptValue.CreateObject();
                ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.CreateObject))));
                ilProcessor.Emit(OpCodes.Stloc_S, propertyDescriptor);

                // [field]PropertyDescriptor.SetProperty(JavaScriptPropertyId.FromString("get"), getter, true);
                ilProcessor.Emit(OpCodes.Ldloca_S, propertyDescriptor);
                ilProcessor.Emit(OpCodes.Ldstr, "get");
                ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(JavaScriptPropertyId).GetMethod(nameof(JavaScriptPropertyId.FromString))));
                ilProcessor.Emit(OpCodes.Ldloc, getterVariable);
                ilProcessor.Emit(OpCodes.Ldc_I4_1);
                ilProcessor.Emit(OpCodes.Call, setPropertyMethod);
                ilProcessor.Emit(OpCodes.Nop);

                // [field]PropertyDescriptor.SetProperty(JavaScriptPropertyId.FromString("set"), setter, true);
                ilProcessor.Emit(OpCodes.Ldloca_S, propertyDescriptor);
                ilProcessor.Emit(OpCodes.Ldstr, "set");
                ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(JavaScriptPropertyId).GetMethod(nameof(JavaScriptPropertyId.FromString))));
                ilProcessor.Emit(OpCodes.Ldloc, setterVariable);
                ilProcessor.Emit(OpCodes.Ldc_I4_1);
                ilProcessor.Emit(OpCodes.Call, setPropertyMethod);
                ilProcessor.Emit(OpCodes.Nop);

                // value.DefineProperty(JavaScriptPropertyId.FromString("[field]"), [field]PropertyDescriptor);
                ilProcessor.Emit(OpCodes.Ldloca_S, prototypeVariable);
                ilProcessor.Emit(OpCodes.Ldstr, field);
                ilProcessor.Emit(OpCodes.Call, javaScriptPropertyFromStringMethod);
                ilProcessor.Emit(OpCodes.Ldloc_S, propertyDescriptor);
                ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.DefineProperty))));
                ilProcessor.Emit(OpCodes.Pop);
            }

            var firstInstructionAfterFinalizerCondition = Instruction.Create(OpCodes.Ldarg_1);
            var endAssignment = Instruction.Create(OpCodes.Ldloca_S, javascriptValue);

            // if (finalize == true)
            ilProcessor.Append(firstInstructionAfterCondition);
            ilProcessor.Emit(OpCodes.Brfalse, firstInstructionAfterFinalizerCondition);

            // var javaScriptValue = JavaScriptValue.CreateExternalObject((IntPtr)ptr, Finalizer);
            ilProcessor.Emit(OpCodes.Ldarg_1);
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(IntPtr).GetMethod("op_Explicit", new[] { typeof(void*) })));
            ilProcessor.Emit(OpCodes.Ldnull);
            ilProcessor.Emit(OpCodes.Ldftn, finalizerMethod);
            ilProcessor.Emit(OpCodes.Newobj, mainModule.ImportReference(typeof(Action<IntPtr>).GetConstructor(new[] { typeof(object), typeof(IntPtr) })));
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.CreateExternalObject))));
            ilProcessor.Emit(OpCodes.Stloc_S, javascriptValue);
            ilProcessor.Emit(OpCodes.Br, endAssignment);

            // var javaScriptValue = JavaScriptValue.CreateExternalObject((IntPtr)ptr, NoopFinalizer);
            ilProcessor.Append(firstInstructionAfterFinalizerCondition);
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(IntPtr).GetMethod("op_Explicit", new[] { typeof(void*) })));
            ilProcessor.Emit(OpCodes.Ldnull);
            ilProcessor.Emit(OpCodes.Ldftn, noopFinalizerMethod);
            ilProcessor.Emit(OpCodes.Newobj, mainModule.ImportReference(typeof(Action<IntPtr>).GetConstructor(new[] { typeof(object), typeof(IntPtr) })));
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.CreateExternalObject))));
            ilProcessor.Emit(OpCodes.Stloc_S, javascriptValue);

            // javaScriptValue.Prototype = prototype;
            ilProcessor.Append(endAssignment);
            ilProcessor.Emit(OpCodes.Ldloc_S, prototypeVariable);
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

        private static MethodDefinition CreateReadComponentTypePropertyGetMethod(TypeReference type, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition(
                "get_ReadComponentType",
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.NewSlot,
                mainModule.ImportReference(typeof(ComponentType)));

            var ilProcessor = method.Body.GetILProcessor();

            var componentTypeMethod = mainModule
                .ImportReference(typeof(ComponentType).GetMethod(nameof(ComponentType.ReadOnly), new Type[] { }))
                .SetGenericParameter(type);

            ilProcessor.Emit(OpCodes.Call, componentTypeMethod);
            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodDefinition CreateComponentSizePropertyGetMethod(TypeReference type, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition(
                "get_ComponentSize",
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.NewSlot,
                mainModule.TypeSystem.Int32);

            var ilProcessor = method.Body.GetILProcessor();

            var sizeOfMethod = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.SizeOf), new Type[] { }))
                .SetGenericParameter(type);

            // return UnsafeUtility.SizeOf<CounterComponent>();
            ilProcessor.Emit(OpCodes.Call, sizeOfMethod);
            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodDefinition AddCreateConstructorMethod(MethodDefinition constructorMethod, ModuleDefinition mainModule)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));

            var method = new MethodDefinition("CreateConstructor", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final,
                javascriptValueTypeDefinition);

            var ilProcessor = method.Body.GetILProcessor();

            var javaScriptNativeFunctionCtor =
                mainModule.ImportReference(typeof(JavaScriptNativeFunction).GetConstructor(new [] { typeof(object), typeof(IntPtr) }));

            var createFunctionMethod = mainModule.ImportReference(typeof(JavaScriptValue).GetMethod("CreateFunction", new[]
            {
                typeof(string),
                typeof(JavaScriptNativeFunction)
            }));

            // return JavaScriptValue.CreateFunction(Constructor);
            ilProcessor.Emit(OpCodes.Ldstr, "ctor");
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
            ilProcessor.Emit(OpCodes.Ldc_I4_1);
            ilProcessor.Emit(OpCodes.Call, createJsObjectForNativeMethod);
            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodDefinition CreateFinalizerMethod(ModuleDefinition mainModule)
        {
            var voidTypeDefinition = mainModule.TypeSystem.Void;

            var method = new MethodDefinition("Finalizer", MethodAttributes.Private | MethodAttributes.Static, voidTypeDefinition);

            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.IntPtr));

            var ilProcessor = method.Body.GetILProcessor();

            // UnsafeUtility.Free((void*)data, Allocator.Persistent);
            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(IntPtr).GetMethod("op_Explicit", new[] { typeof(void*) })));
            ilProcessor.Emit(OpCodes.Ldc_I4_4);
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.Free), new[] { typeof(void*), typeof(Allocator) })));

            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }

        private static MethodDefinition CreateNoopFinalizerMethod(ModuleDefinition mainModule)
        {
            var voidTypeDefinition = mainModule.TypeSystem.Void;

            var method = new MethodDefinition("NoopFinalizer", MethodAttributes.Private | MethodAttributes.Static, voidTypeDefinition);

            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.IntPtr));

            var ilProcessor = method.Body.GetILProcessor();

            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }
    }
}