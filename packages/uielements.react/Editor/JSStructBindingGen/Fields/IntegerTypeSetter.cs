using System;
using System.Collections.Generic;
using ChakraHost.Hosting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen.Fields
{
    internal class IntegerTypeSetter : ITypeSetter
    {
        public bool Matches(TypeReference type, IEnumerable<TypeReference> supportedStructureTypes)
        {
            return type.Name == nameof(Int32) ||
                   type.Name == nameof(Int16) ||
                   type.Name == nameof(UInt32) ||
                   type.Name == nameof(UInt16) ||
                   type.Name == nameof(Byte);
        }

        public void EmitSetter(
            FieldReference field,
            VariableDefinition structure,
            Collection<VariableDefinition> variables,
            ModuleDefinition mainModule,
            ILProcessor ilProcessor)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));
            var toInt32Method =
                mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.ToInt32)));

            // str.count = arguments[1].ToInt32();
            ilProcessor.Emit(OpCodes.Ldloca_S, structure);
            ilProcessor.Emit(OpCodes.Ldarg_3);
            ilProcessor.Emit(OpCodes.Ldc_I4_1);
            ilProcessor.Emit(OpCodes.Ldelema, javascriptValueTypeDefinition);
            ilProcessor.Emit(OpCodes.Call, toInt32Method);
            ilProcessor.Emit(OpCodes.Stfld, field);
        }
    }
}