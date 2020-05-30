using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityReactUIElements;

namespace Unity.ReactUIElements.JsStructBinding.CodeGen.Fields
{
    internal class StructureTypeGetter : ITypeGetter
    {
        public bool Matches(TypeReference type, IEnumerable<TypeReference> supportedStructureTypes)
        {
            return supportedStructureTypes.Any(e => TypeUtils.IsSameType(e, type));
        }

        public void EmitGetter(
            FieldReference field,
            VariableDefinition structure,
            Collection<VariableDefinition> variables,
            ModuleDefinition mainModule,
            ILProcessor ilProcessor)
        {
            var fieldType = field.FieldType.Module != mainModule
                ? mainModule.ImportReference(field.FieldType)
                : field.FieldType;

            var nestedFactory = new VariableDefinition(mainModule.ImportReference(typeof(IJSBindingFactory)));
            var size = new VariableDefinition(mainModule.TypeSystem.Int32);
            var alignment = new VariableDefinition(mainModule.TypeSystem.Int32);
            var ptr = new VariableDefinition(mainModule.TypeSystem.Void.MakePointerType());

            variables.Add(nestedFactory);
            variables.Add(size);
            variables.Add(alignment);
            variables.Add(ptr);

            var getFactoryMethod =
                mainModule.ImportReference(
                    typeof(JSTypeFactories).GetMethod(nameof(JSTypeFactories.GetFactory)));

            var sizeOfMethod = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.SizeOf), new Type[] { }))
                .SetGenericParameter(fieldType);

            var alignOfMethod = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.AlignOf), new Type[] { }))
                .SetGenericParameter(fieldType);

            var copyStructureToPtrMethod = mainModule
                .ImportReference(typeof(UnsafeUtility).GetMethod(nameof(UnsafeUtility.CopyStructureToPtr)))
                .SetGenericParameter(fieldType);

            var createJsObjectForNativeMethod = mainModule.ImportReference(
                typeof(IJSBindingFactory).GetMethod(nameof(IJSBindingFactory.CreateJsObjectForNative)));

            // var nestedFactory = JSTypeFactories.GetFactory("SomeNestedStruct");
            ilProcessor.Emit(OpCodes.Ldstr, fieldType.Name);
            ilProcessor.Emit(OpCodes.Call, getFactoryMethod);
            ilProcessor.Emit(OpCodes.Stloc_S, nestedFactory);

            // var size = UnsafeUtility.SizeOf<SomeNestedStruct>();
            ilProcessor.Emit(OpCodes.Call, sizeOfMethod);
            ilProcessor.Emit(OpCodes.Stloc_S, size);

            // var alignment = UnsafeUtility.AlignOf<SomeNestedStruct>();
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

            // UnsafeUtility.CopyStructureToPtr(ref str.Nested, ptr);
            ilProcessor.Emit(OpCodes.Ldloca_S, structure);
            ilProcessor.Emit(OpCodes.Ldflda, field);
            ilProcessor.Emit(OpCodes.Ldloc_S, ptr);
            ilProcessor.Emit(OpCodes.Call, copyStructureToPtrMethod);

            // return nestedFactory.CreateJsObjectForNative(ptr);
            ilProcessor.Emit(OpCodes.Ldloc_S, nestedFactory);
            ilProcessor.Emit(OpCodes.Ldloc_S, ptr);
            ilProcessor.Emit(OpCodes.Callvirt, createJsObjectForNativeMethod);
            ilProcessor.Emit(OpCodes.Ret);
        }
    }
}