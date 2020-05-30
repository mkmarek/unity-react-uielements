using System;
using System.Collections.Generic;
using System.Linq;
using ChakraHost.Hosting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using Unity.Collections;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen.Fields
{
    internal class StringTypeGetter : ITypeGetter
    {
        public bool Matches(TypeReference type, IEnumerable<TypeReference> supportedStructureTypes)
        {
            return type.Name == nameof(FixedString32) ||
                   type.Name == nameof(FixedString64) ||
                   type.Name == nameof(FixedString128) ||
                   type.Name == nameof(FixedString512) ||
                   type.Name == nameof(FixedString4096);
        }

        public void EmitGetter(
            FieldReference field,
            VariableDefinition structure,
            Collection<VariableDefinition> variables,
            ModuleDefinition mainModule,
            ILProcessor ilProcessor)
        {
            var strVariable = new VariableDefinition(mainModule.TypeSystem.String);
            variables.Add(strVariable);

            var fieldType = mainModule.ImportReference(field.FieldType);
            var toStringMethod = mainModule.ImportReference(mainModule.TypeSystem.Object.Resolve().Methods
                .FirstOrDefault(e => e.Name == "ToString"));
            var fromStringMethod =
                mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.FromString)));

            // return JavaScriptValue.FromString(str.[field]);
            ilProcessor.Emit(OpCodes.Ldloca_S, structure);
            ilProcessor.Emit(OpCodes.Ldflda, field);
            ilProcessor.Emit(OpCodes.Constrained, fieldType);
            ilProcessor.Emit(OpCodes.Callvirt, toStringMethod);
            ilProcessor.Emit(OpCodes.Stloc_S, strVariable);
            ilProcessor.Emit(OpCodes.Nop);

            ilProcessor.Emit(OpCodes.Ldloc_S, strVariable);
            ilProcessor.Emit(OpCodes.Call, fromStringMethod);
            ilProcessor.Emit(OpCodes.Ret);
        }
    }
}