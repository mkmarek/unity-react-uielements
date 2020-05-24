using System;
using System.Collections.Generic;
using ChakraHost.Hosting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen.Fields
{
    internal class FloatTypeGetter : ITypeGetter
    {
        public bool Matches(TypeReference type, IEnumerable<TypeReference> supportedStructureTypes)
        {
            return type.Name == nameof(Single) ||
                   type.Name == nameof(Double);
        }

        public void EmitGetter(
            FieldReference field,
            VariableDefinition structure,
            Collection<VariableDefinition> variables,
            ModuleDefinition mainModule,
            ILProcessor ilProcessor)
        {
            var fromDoubleMethod =
                mainModule.ImportReference(typeof(JavaScriptValue).GetMethod(nameof(JavaScriptValue.FromDouble)));

            // return JavaScriptValue.FromDouble(str.[field]);
            ilProcessor.Emit(OpCodes.Ldloca_S, structure);
            ilProcessor.Emit(OpCodes.Ldfld, field);
            ilProcessor.Emit(OpCodes.Call, fromDoubleMethod);
            ilProcessor.Emit(OpCodes.Ret);
        }
    }
}