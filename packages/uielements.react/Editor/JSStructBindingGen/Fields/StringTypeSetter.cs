using ChakraHost.Hosting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen.Fields
{
    internal class StringTypeSetter : ITypeSetter
    {
        public bool Matches(TypeReference type, IEnumerable<TypeReference> supportedStructureTypes)
        {
            return type.Name == nameof(FixedString32) ||
                   type.Name == nameof(FixedString64) ||
                   type.Name == nameof(FixedString128) ||
                   type.Name == nameof(FixedString512) ||
                   type.Name == nameof(FixedString4096);
        }

        public void EmitSetter(
            FieldReference field,
            VariableDefinition structure,
            Collection<VariableDefinition> variables,
            ModuleDefinition mainModule,
            ILProcessor ilProcessor)
        {
            var javascriptValueTypeDefinition = mainModule.ImportReference(typeof(JavaScriptValue));
            var stringFieldType = mainModule.ImportReference(field.FieldType);
            var toStringImplicit = mainModule.ImportReference(stringFieldType.Resolve()
                .Methods.FirstOrDefault(e => e.Name == "op_Implicit" && e.Parameters[0].ParameterType.Name.ToLower() == "string"));
            var toStringMethod =
                mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.ToString)));

            var stringValueVariable = new VariableDefinition(mainModule.TypeSystem.String);
            variables.Add(stringValueVariable);

            //var strValue = arguments[1].ToString();
            ilProcessor.Emit(OpCodes.Ldarg_2);
            ilProcessor.Emit(OpCodes.Ldc_I4_1);
            ilProcessor.Emit(OpCodes.Ldelema, javascriptValueTypeDefinition);
            ilProcessor.Emit(OpCodes.Call, toStringMethod);
            ilProcessor.Emit(OpCodes.Stloc_S, stringValueVariable);

            // str.count = (string)strValue;
            ilProcessor.Emit(OpCodes.Ldloca_S, structure);
            ilProcessor.Emit(OpCodes.Ldloc_S, stringValueVariable);
            ilProcessor.Emit(OpCodes.Call, toStringImplicit);
            ilProcessor.Emit(OpCodes.Stfld, field);
        }
    }
}