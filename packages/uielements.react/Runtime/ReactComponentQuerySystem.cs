using ChakraHost.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

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
            var ptr = (byte*) DataPtr;

            for (var i = 0; i < chunk.Count; i++)
            {
                var copySizeInBytes = UnsafeUtility.SizeOf<T>();
                var destinationPtr = ptr + (StepSize * (i + entityOffset)) + SlotOffset + ReactComponentQuerySystem.HeaderDataOffset;
                var srcPtr = (byte*)arrayPtr + copySizeInBytes * i;

                UnsafeUtility.MemCpy(destinationPtr, srcPtr, copySizeInBytes);
            }
        }
    }

    [BurstCompile]
    public unsafe struct CopyEntitiesToSlotsAndIncrementVersion : IJobChunk
    {
        [NativeDisableUnsafePtrRestriction]
        public void* DataPtr;

        public int StepSize;

        public int SlotOffset;

        [ReadOnly]
        public ArchetypeChunkEntityType EntityType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int entityOffset)
        {
            var arrayPtr = chunk.GetNativeArray(EntityType).GetUnsafeReadOnlyPtr();
            var versionPointer = (int*) DataPtr;
            var ptr = (byte*) DataPtr;

            (*versionPointer)++;

            for (var i = 0; i < chunk.Count; i++)
            {
                var copySizeInBytes = UnsafeUtility.SizeOf<Entity>();
                var destinationPtr = ptr + (StepSize * (i + entityOffset)) + SlotOffset + ReactComponentQuerySystem.HeaderDataOffset;
                var srcPtr = (byte*)arrayPtr + copySizeInBytes * i;

                UnsafeUtility.MemCpy(destinationPtr, srcPtr, copySizeInBytes);
            }
        }
    }

    public abstract class ReactComponentQuerySystem : JobComponentSystem
    {
        public const int HeaderDataOffset = sizeof(int);

        public unsafe class QueryData : IDisposable
        {
            public bool HasData;
            public int Size;
            public void* DataPtr;
            public int SlotSize;
            public Dictionary<string, int> OffsetMap;
            public EntityQuery Query;
            public string[] Components;
            public int PreviousVersion;
            public JavaScriptValue OnChangeCallback;
            public ComponentType[] ComponentTypes;

            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                if (!IsDisposed)
                {
                    OnChangeCallback.Release();

                    UnsafeUtility.Free(DataPtr, Allocator.Persistent);

                    HasData = false;
                    Size = 0;
                }

                IsDisposed = true;
            }
        }

        private Dictionary<string, QueryData> queryRefs = new Dictionary<string, QueryData>();

        public JavaScriptValue CreateQuery(string[] components, JavaScriptValue callback)
        {
            Array.Sort(components, StringComparer.InvariantCulture);
            var key = string.Join("_", components);

            if (!queryRefs.ContainsKey(key))
            {
                var componentTypes = new ComponentType[components.Length];
                var offsetMap = new Dictionary<string, int>();

                //First slot is designated for entity reference
                offsetMap.Add("Entity", 0);
                var offset = UnsafeUtility.SizeOf<Entity>();

                for (var i = 0; i < components.Length; i++)
                {
                    var factory = JSTypeFactories.GetFactory(components[i]);

                    componentTypes[i] = factory.ReadComponentType;
                    offsetMap.Add(components[i], offset);

                    offset += factory.ComponentSize;
                }

                var q = GetEntityQuery(componentTypes);
                q.ResetFilter();

                callback.AddRef();

                queryRefs.Add(key, new QueryData
                {
                    HasData = false,
                    Query = q,
                    Components = components,
                    OffsetMap = offsetMap,
                    SlotSize = offset,
                    OnChangeCallback = callback,
                    ComponentTypes = componentTypes
                });
            }

            var getSizeFunction = JavaScriptValue.CreateFunction("getSize", (callee, call, arguments, count, data) =>
            {
                var currentQueryData = arguments[0].ObjectFromJavaScriptValue<QueryData>();

                return JavaScriptValue.FromInt32(currentQueryData.Size);
            });

            var getVersionFunction = JavaScriptValue.CreateFunction("getVersion", (callee, call, arguments, count, data) =>
            {
                var currentQueryData = arguments[0].ObjectFromJavaScriptValue<QueryData>();

                unsafe
                {
                    var ptr = (int*)currentQueryData.DataPtr;

                    return JavaScriptValue.FromInt32(*ptr);
                }
            });

            var getHasDataFunction = JavaScriptValue.CreateFunction("getHasData", (callee, call, arguments, count, data) =>
            {
                var currentQueryData = arguments[0].ObjectFromJavaScriptValue<QueryData>();

                return JavaScriptValue.FromBoolean(currentQueryData.HasData);
            });

            var getElementAtFunction = JavaScriptValue.CreateFunction("getElementAt", (callee, call, arguments, count, data) =>
            {
                var currentQueryData = arguments[0].ObjectFromJavaScriptValue<QueryData>();

                if (arguments.Length < 3 ||
                    arguments[1].ValueType != JavaScriptValueType.String ||
                    arguments[2].ValueType != JavaScriptValueType.Number ||
                    currentQueryData == null ||
                    currentQueryData.IsDisposed ||
                    !currentQueryData.HasData)
                {
                    return JavaScriptValue.Null;
                }

                var slot = arguments[1].ToString();

                if (!currentQueryData.OffsetMap.ContainsKey(slot)) return JavaScriptValue.Null;

                if (arguments[2].ToInt32() >= currentQueryData.Size)
                {
                    return JavaScriptValue.Null;
                }

                var indexInArray = currentQueryData.SlotSize * arguments[2].ToInt32();
                var slotIndex = currentQueryData.OffsetMap[slot];
                var factory = JSTypeFactories.GetFactory(slot);

                if (!JavaScriptContext.Current.IsValid)
                {
                    return JavaScriptValue.Null;
                }

                unsafe
                {
                    var ptr = (byte*)currentQueryData.DataPtr + HeaderDataOffset + indexInArray + slotIndex;

                    var output = UnsafeUtility.Malloc(factory.ComponentSize, 4, Allocator.Persistent);
                    UnsafeUtility.MemCpy(output, ptr, factory.ComponentSize);

                    return factory.CreateJsObjectForNative(output, true);
                }
            });

            var disposeFunction = JavaScriptValue.CreateFunction("dispose", (callee, call, arguments, count, data) =>
            {
                var currentQueryData = arguments[0].ObjectFromJavaScriptValue<QueryData>();

                currentQueryData.Dispose();

                return JavaScriptValue.Undefined;
            });

            var queryData = queryRefs[key];
            var handle = GCHandle.Alloc(queryData);
            var p = GCHandle.ToIntPtr(handle);

            var ret = JavaScriptValue.CreateExternalObject(p, Finalizer);
            ret.SetProperty(JavaScriptPropertyId.FromString("getSize"), getSizeFunction, true);
            ret.SetProperty(JavaScriptPropertyId.FromString("hasData"), getHasDataFunction, true);
            ret.SetProperty(JavaScriptPropertyId.FromString("getElementAt"), getElementAtFunction, true);
            ret.SetProperty(JavaScriptPropertyId.FromString("dispose"), disposeFunction, true);
            ret.SetProperty(JavaScriptPropertyId.FromString("getVersion"), getVersionFunction, true);

            return ret;
        }

        private static void Finalizer(IntPtr data)
        {
            if (data != IntPtr.Zero)
            {
                var handle = GCHandle.FromIntPtr(data);
                var queryData = (QueryData) handle.Target;

                queryData.Dispose();
                handle.Free();
            }
        }

        protected override void OnCreate()
        {
            queryRefs = new Dictionary<string, QueryData>();

            base.OnCreate();
        }
        // This method will be overriden by the code generator
        protected abstract JobHandle ScheduleJobForComponent(
            QueryData queryRef,
            string componentType,
            JobHandle inputDeps);

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var combined = inputDeps;
            var valuesCopy = queryRefs.ToList();

            foreach (var queryRef in valuesCopy)
            {
                bool wasReset = false;

                if (queryRef.Value == null || queryRef.Value.IsDisposed)
                {
                    queryRefs.Remove(queryRef.Key);
                    continue;
                }

                unsafe
                {
                    // Check if version changed
                    if (queryRef.Value.HasData)
                    {
                        var versionPtr = (int*) queryRef.Value.DataPtr;
                        var currentVersion = *versionPtr;

                        if (currentVersion != queryRef.Value.PreviousVersion)
                        {
                            if (queryRef.Value.OnChangeCallback.IsValid && queryRef.Value.OnChangeCallback.ValueType ==
                                JavaScriptValueType.Function)
                            {
                                queryRef.Value.OnChangeCallback.CallFunction(JavaScriptValue.Null);
                            }

                            queryRef.Value.PreviousVersion = currentVersion;
                        }
                    }
                }

                var size = queryRef.Value.Query.CalculateEntityCountWithoutFiltering();

                if (!queryRef.Value.HasData || queryRef.Value.Size != size)
                {
                    wasReset = true;

                    unsafe
                    {
                        if (queryRef.Value.HasData)
                        {
                            UnsafeUtility.Free(queryRef.Value.DataPtr, Allocator.Persistent);
                        }

                        queryRef.Value.DataPtr =
                            UnsafeUtility.Malloc(HeaderDataOffset + size * queryRef.Value.SlotSize, 4, Allocator.Persistent);

                        queryRef.Value.HasData = true;
                        queryRef.Value.Size = size;
                    }
                }

                unsafe
                {
                    var copyEntities = new CopyEntitiesToSlotsAndIncrementVersion
                    {
                        DataPtr = queryRef.Value.DataPtr,
                        StepSize = queryRef.Value.SlotSize,
                        EntityType = GetArchetypeChunkEntityType(),
                        SlotOffset = queryRef.Value.OffsetMap["Entity"]
                    };

                    combined = copyEntities.Schedule(queryRef.Value.Query, combined);
                }

                foreach (var component in queryRef.Value.Components)
                {
                    combined = JobHandle.CombineDependencies(combined,
                        ScheduleJobForComponent(queryRef.Value, component, inputDeps));
                }

                if (wasReset) queryRef.Value.Query.SetChangedVersionFilter(queryRef.Value.ComponentTypes);
            }

            return combined;
        }
    }
}