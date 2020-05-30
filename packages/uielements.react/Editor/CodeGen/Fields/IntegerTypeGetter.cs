using System;
using System.Collections.Generic;
using ChakraHost.Hosting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen.Fields
{
    internal class IntegerTypeGetter : ITypeGetter
    {
        public bool Matches(TypeReference type, IEnumerable<TypeReference> supportedStructureTypes)
        {
            return type.Name == nameof(Int32) ||
                   type.Name == nameof(Int16) ||
                   type.Name == nameof(UInt32) ||
                   type.Name == nameof(UInt16) ||
                   type.Name == nameof(Byte);
        }

        public void EmitGetter(
            FieldReference field,
            VariableDefinition structure,
            Collection<VariableDefinition> variables,
            ModuleDefinition mainModule,
            ILProcessor ilProcessor)
        {
            var fromInt32Method =
                mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.FromInt32)));

            ilProcessor.Emit(OpCodes.Ldloca_S, structure);
            ilProcessor.Emit(OpCodes.Ldfld, field);
            ilProcessor.Emit(OpCodes.Call, fromInt32Method);
            ilProcessor.Emit(OpCodes.Ret);
        }
    }
}