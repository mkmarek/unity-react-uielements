using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen.Fields
{
    internal interface ITypeSetter
    {
        bool Matches(TypeReference type, IEnumerable<TypeReference> supportedStructureTypes);
        void EmitSetter(
            FieldReference field,
            VariableDefinition structure,
            Collection<VariableDefinition> variables,
            ModuleDefinition mainModule,
            ILProcessor ilProcessor);
    }
}