using System;
using System.Collections.Generic;
using System.Linq;
using ChakraHost.Hosting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen.Fields
{
    internal class StructureTypeSetter : ITypeSetter
    {
        public bool Matches(TypeReference type, IEnumerable<TypeReference> supportedStructureTypes)
        {
            return supportedStructureTypes.Any(e => TypeUtils.IsSameType(e, type));
        }

        public void EmitSetter(
            FieldReference field,
            VariableDefinition structure,
            Collection<VariableDefinition> variables,
            ModuleDefinition mainModule,
            ILProcessor ilProcessor)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));
            var fieldType = field.FieldType.Module != mainModule
                ? mainModule.ImportReference(field.FieldType)
                : field.FieldType;

            var copyPtrToStructure = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.CopyPtrToStructure)))
                .SetGenericParameter(fieldType);

            var data1 = new VariableDefinition(mainModule.TypeSystem.Void.MakePointerType());
            var fieldValue = new VariableDefinition(fieldType);

            variables.Add(data1);
            variables.Add(fieldValue);

            // var data1 = (void*)arguments[1].ExternalData;
            ilProcessor.Emit(OpCodes.Ldarg_2);
            ilProcessor.Emit(OpCodes.Ldc_I4_1);
            ilProcessor.Emit(OpCodes.Ldelema, javascriptValueTypeDefinition);
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(JavaScriptValue).GetMethod("get_ExternalData")));
            ilProcessor.Emit(OpCodes.Call, mainModule.ImportReference(typeof(IntPtr).GetMethod("op_Explicit", new[] { typeof(IntPtr) }, typeof(void*))));
            ilProcessor.Emit(OpCodes.Stloc_S, data1);

            // unsafeUtility.CopyPtrToStructure<SomeNestedStruct>(data1, out var field);
            ilProcessor.Emit(OpCodes.Ldloc_S, data1);
            ilProcessor.Emit(OpCodes.Ldloca_S, fieldValue);
            ilProcessor.Emit(OpCodes.Call, copyPtrToStructure);
            ilProcessor.Emit(OpCodes.Nop);

            // str.Nested = field;
            ilProcessor.Emit(OpCodes.Ldloca_S, structure);
            ilProcessor.Emit(OpCodes.Ldloc_S, fieldValue);
            ilProcessor.Emit(OpCodes.Stfld, field);
        }
    }
}