using ChakraHost.Hosting;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace UnityReactUIElements
{
    [BurstCompile]
    public unsafe struct CopyComponentDataToSlots<T> : IJobChunk
        where T : struct, IComponentData
    {
        [NativeDisableUnsafePtrRestriction]
        public void* DataPtr;

        public int StepSize;

        public int SlotOffset;

        [ReadOnly]
        public ArchetypeChunkComponentType<T> ComponentType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int entityOffset)
        {
            var arrayPtr = chunk.GetNativeArray(ComponentType).GetUnsafeReadOnlyPtr();

            for (var i = 0; i < chunk.Count; i++)
            {
                var copySizeInBytes = UnsafeUtility.SizeOf<T>();
                var destinationPtr = (byte*) DataPtr + (StepSize * (i + entityOffset)) + SlotOffset;
                var srcPtr = (byte*)arrayPtr + copySizeInBytes * i;

                UnsafeUtility.MemCpy(destinationPtr, srcPtr, copySizeInBytes);
            }
        }
    }

    public interface IJSBindingFactory
    {
        string Name { get; }
        ComponentType ReadComponentType { get; }
        int ComponentSize { get; }

        JavaScriptValue CreateConstructor();
        unsafe JavaScriptValue CreateJsObjectForNative(void* ptr, bool finalize);
    }
}
