using System;
using System.Collections.Generic;
using ChakraHost.Hosting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen.Fields
{
    public static class FieldProcessor
    {
        private static readonly ITypeSetter[] Setters = {
            new FloatTypeSetter(),
            new IntegerTypeSetter(),
            new StructureTypeSetter()
        };

        private static readonly ITypeGetter[] Getters = {
            new FloatTypeGetter(),
            new IntegerTypeGetter(),
            new StructureTypeGetter()
        };

        public static MethodDefinition CreateSetter(
            TypeReference componentType,
            FieldReference field,
            ModuleDefinition mainModule,
            IEnumerable<TypeReference> supportedStructureTypes)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));
            var javascriptValueArrayTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue[]));
            var getExternalDataMethod =
                mainModule.ImportReference(typeof(JavaScriptValue).GetMethod("get_ExternalData"));
            var getUndefinedMethod = mainModule.ImportReference(typeof(JavaScriptValue).GetMethod("get_Undefined"));
            var ptrToVoidPointerExplicit =
                mainModule.ImportReference(typeof(IntPtr).GetMethod("op_Explicit", new[] {typeof(IntPtr)},
                    typeof(void*)));

            var copyPtrToStructure = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.CopyPtrToStructure)))
                .SetGenericParameter(componentType);

            var copyStructureToPtr = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.CopyStructureToPtr)))
                .SetGenericParameter(componentType);

            var method = new MethodDefinition($"Set_{field.Name}", MethodAttributes.Private | MethodAttributes.Static, javascriptValueTypeDefinition);

            method.Parameters.Add(new ParameterDefinition(javascriptValueTypeDefinition));
            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.Boolean));
            method.Parameters.Add(new ParameterDefinition(javascriptValueArrayTypeDefinition));
            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.Int16));
            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.IntPtr));

            var ilProcessor = method.Body.GetILProcessor();

            var externalData = new VariableDefinition(mainModule.TypeSystem.Void.MakePointerType());
            var str = new VariableDefinition(componentType);

            method.Body.Variables.Add(externalData);
            method.Body.Variables.Add(str);

            // var data = (void*)arguments[0].ExternalData;
            ilProcessor.Emit(OpCodes.Ldarg_2);
            ilProcessor.Emit(OpCodes.Ldc_I4_0);
            ilProcessor.Emit(OpCodes.Ldelema, javascriptValueTypeDefinition);
            ilProcessor.Emit(OpCodes.Call, getExternalDataMethod);
            ilProcessor.Emit(OpCodes.Call, ptrToVoidPointerExplicit);
            ilProcessor.Emit(OpCodes.Stloc_0);

            // UnsafeUtility.CopyPtrToStructure<CounterComponent>(data, out var str);
            ilProcessor.Emit(OpCodes.Ldloc_0);
            ilProcessor.Emit(OpCodes.Ldloca_S, str);
            ilProcessor.Emit(OpCodes.Call, copyPtrToStructure);
            ilProcessor.Emit(OpCodes.Nop);

            EmitSetter(field, str, method.Body.Variables, mainModule, ilProcessor, supportedStructureTypes);

            // UnsafeUtility.CopyStructureToPtr(ref str, data);
            ilProcessor.Emit(OpCodes.Ldloca_S, str);
            ilProcessor.Emit(OpCodes.Ldloc_0);
            ilProcessor.Emit(OpCodes.Call, copyStructureToPtr);
            ilProcessor.Emit(OpCodes.Nop);

            // return JavaScriptValue.Undefined;
            ilProcessor.Emit(OpCodes.Call, getUndefinedMethod);
            ilProcessor.Emit(OpCodes.Ret);

            return method;
        }

        private static void EmitSetter(
            FieldReference field,
            VariableDefinition structure,
            Collection<VariableDefinition> variables,
            ModuleDefinition mainModule,
            ILProcessor ilProcessor,
            IEnumerable<TypeReference> supportedStructureTypes)
        {
            var emitted = false;

            foreach (var setter in Setters)
            {
                if (setter.Matches(field.FieldType, supportedStructureTypes))
                {
                    setter.EmitSetter(field, structure, variables, mainModule, ilProcessor);
                    emitted = true;
                    break;
                }
            }

            if (!emitted)
            {
                throw new NotSupportedException($"Field {field.Name} of type {field.FieldType.Name} is not supported");
            }
        }

        public static MethodDefinition CreateGetter(
            TypeReference componentType,
            FieldReference field,
            ModuleDefinition mainModule,
            IEnumerable<TypeReference> supportedStructureTypes)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));
            var javascriptValueArrayTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue[]));
            var getExternalDataMethod = mainModule.ImportReference(typeof(JavaScriptValue).GetMethod("get_ExternalData"));
            var ptrToVoidPointerExplicit =
                mainModule.ImportReference(typeof(IntPtr).GetMethod("op_Explicit", new[] {typeof(IntPtr)},
                    typeof(void*)));
            var copyPtrToStructure = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.CopyPtrToStructure)))
                .SetGenericParameter(componentType);

            var method = new MethodDefinition($"Get_{field.Name}", MethodAttributes.Private | MethodAttributes.Static, javascriptValueTypeDefinition);

            method.Parameters.Add(new ParameterDefinition(javascriptValueTypeDefinition));
            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.Boolean));
            method.Parameters.Add(new ParameterDefinition(javascriptValueArrayTypeDefinition));
            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.Int16));
            method.Parameters.Add(new ParameterDefinition(mainModule.TypeSystem.IntPtr));

            var ilProcessor = method.Body.GetILProcessor();

            var externalData = new VariableDefinition(mainModule.TypeSystem.Void.MakePointerType());
            var str = new VariableDefinition(componentType);

            method.Body.Variables.Add(externalData);
            method.Body.Variables.Add(str);

            // var data = (void*)arguments[0].ExternalData;
            ilProcessor.Emit(OpCodes.Ldarg_2);
            ilProcessor.Emit(OpCodes.Ldc_I4_0);
            ilProcessor.Emit(OpCodes.Ldelema, javascriptValueTypeDefinition);
            ilProcessor.Emit(OpCodes.Call, getExternalDataMethod);
            ilProcessor.Emit(OpCodes.Call, ptrToVoidPointerExplicit);
            ilProcessor.Emit(OpCodes.Stloc_0);

            // UnsafeUtility.CopyPtrToStructure<CounterComponent>(data, out var str);
            ilProcessor.Emit(OpCodes.Ldloc_0);
            ilProcessor.Emit(OpCodes.Ldloca_S, str);
            ilProcessor.Emit(OpCodes.Call, copyPtrToStructure);
            ilProcessor.Emit(OpCodes.Nop);

            EmitGetter(field, str, method.Body.Variables, mainModule, ilProcessor, supportedStructureTypes);

            return method;
        }

        private static void EmitGetter(
            FieldReference field,
            VariableDefinition structure,
            Collection<VariableDefinition> variables,
            ModuleDefinition mainModule,
            ILProcessor ilProcessor,
            IEnumerable<TypeReference> supportedStructureTypes)
        {
            var emitted = false;

            foreach (var setter in Getters)
            {
                if (setter.Matches(field.FieldType, supportedStructureTypes))
                {
                    setter.EmitGetter(field, structure, variables, mainModule, ilProcessor);
                    emitted = true;
                    break;
                }
            }

            if (!emitted)
            {
                throw new NotSupportedException($"Field {field.Name} of type {field.FieldType.Name} is not supported");
            }
        }
    }
}
