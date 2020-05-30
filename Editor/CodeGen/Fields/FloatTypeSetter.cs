using System;
using System.Collections.Generic;
using ChakraHost.Hosting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen.Fields
{
    internal class FloatTypeSetter : ITypeSetter
    {
        public bool Matches(TypeReference type, IEnumerable<TypeReference> supportedStructureTypes)
        {
            return type.Name == nameof(Single) ||
                   type.Name == nameof(Double);
        }

        public void EmitSetter(
            FieldReference field,
            VariableDefinition structure,
            Collection<VariableDefinition> variables,
            ModuleDefinition mainModule,
            ILProcessor ilProcessor)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));
            var toDoubleMethod =
                mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.ToDouble)));

            // str.count = arguments[1].ToDouble();
            ilProcessor.Emit(OpCodes.Ldloca_S, structure);
            ilProcessor.Emit(OpCodes.Ldarg_2);
            ilProcessor.Emit(OpCodes.Ldc_I4_1);
            ilProcessor.Emit(OpCodes.Ldelema, javascriptValueTypeDefinition);
            ilProcessor.Emit(OpCodes.Call, toDoubleMethod);
            ilProcessor.Emit(OpCodes.Stfld, field);
        }
    }
}