using ChakraHost.Hosting;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

namespace UnityReactUIElements
{
    public abstract class ReactComponentQuerySystem : JobComponentSystem
    {
        public unsafe class QueryData : IDisposable
        {
            public bool HasData;
            public int Size;
            public void* DataPtr;
            public int SlotSize;
            public Dictionary<string, int> OffsetMap;
            public EntityQuery Query;
            public string[] Components;

            public void Dispose()
            {
                if (HasData) UnsafeUtility.Free(DataPtr, Allocator.Persistent);

                HasData = false;
                Size = 0;
            }
        }

        private Dictionary<string, QueryData> queryRefs = new Dictionary<string, QueryData>();

        public JavaScriptValue CreateQuery(string[] components)
        {
            Array.Sort(components, StringComparer.InvariantCulture);
            var key = string.Join("_", components);

            if (!queryRefs.ContainsKey(key))
            {
                var componentTypes = new ComponentType[components.Length];
                var offsetMap = new Dictionary<string, int>();

                //First slot is designated for entity reference
                var offset = UnsafeUtility.SizeOf<Entity>();
                offsetMap.Add("Entity", 0);

                for (var i = 0; i < components.Length; i++)
                {
                    var factory = JSTypeFactories.GetFactory(components[i]);

                    componentTypes[i] = factory.ReadComponentType;
                    offsetMap.Add(components[i], offset);

                    offset += factory.ComponentSize;
                }

                var q = GetEntityQuery(componentTypes);
                //q.SetChangedVersionFilter(componentTypes);

                queryRefs.Add(key, new QueryData
                {
                    HasData = false,
                    Query = q,
                    Components = components,
                    OffsetMap = offsetMap,
                    SlotSize = offset
                });
            }

            var queryData = queryRefs[key];

            var getSizeFunction = JavaScriptValue.CreateFunction((callee, call, arguments, count, data) =>
            {
                var currentQueryData = arguments[0].ObjectFromJavaScriptValue<QueryData>();

                return JavaScriptValue.FromInt32(currentQueryData.Size);
            });

            var getHasDataFunction = JavaScriptValue.CreateFunction((callee, call, arguments, count, data) =>
            {
                var currentQueryData = arguments[0].ObjectFromJavaScriptValue<QueryData>();

                return JavaScriptValue.FromBoolean(currentQueryData.HasData);
            });

            var getElementAtFunction = JavaScriptValue.CreateFunction((callee, call, arguments, count, data) =>
            {
                var currentQueryData = arguments[0].ObjectFromJavaScriptValue<QueryData>();

                if (arguments.Length < 3 ||
                    arguments[1].ValueType != JavaScriptValueType.String ||
                    arguments[2].ValueType != JavaScriptValueType.Number)
                {
                    return JavaScriptValue.Null;
                }

                var slot = arguments[1].ToString();

                if (!currentQueryData.OffsetMap.ContainsKey(slot)) return JavaScriptValue.Null;

                var indexInArray = currentQueryData.Size * arguments[2].ToInt32();
                var slotIndex = currentQueryData.OffsetMap[slot];
                var factory = JSTypeFactories.GetFactory(slot);

                unsafe
                {
                    var ptr = (byte*)currentQueryData.DataPtr;
                    ptr += indexInArray + slotIndex;

                    return factory.CreateJsObjectForNative(ptr, false);
                }
            });

            var ret = queryData.ToJavaScriptValue();
            ret.SetProperty(JavaScriptPropertyId.FromString("getSize"), getSizeFunction, true);
            ret.SetProperty(JavaScriptPropertyId.FromString("hasData"), getHasDataFunction, true);
            ret.SetProperty(JavaScriptPropertyId.FromString("getElementAt"), getElementAtFunction, true);

            return ret;
        }

        protected override void OnCreate()
        {
            queryRefs = new Dictionary<string, QueryData>();

            base.OnCreate();
        }

        protected override unsafe void OnDestroy()
        {
            foreach (var queryRef in queryRefs)
            {
                queryRef.Value?.Dispose();
            }

            base.OnDestroy();
        }

        // This method will be overriden by the code generator
        protected abstract JobHandle ScheduleJobForComponent(
            QueryData queryRef,
            string componentType,
            JobHandle inputDeps);

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var combined = inputDeps;
            foreach (var queryRef in queryRefs.Values)
            {
                foreach (var component in queryRef.Components)
                {
                    var size = queryRef.Query.CalculateEntityCount();

                    if (!queryRef.HasData || queryRef.Size != size)
                    {
                        unsafe
                        {
                            if (queryRef.HasData)
                            {
                                UnsafeUtility.Free(queryRef.DataPtr, Allocator.Persistent);
                            }

                            queryRef.DataPtr = UnsafeUtility.Malloc(size * queryRef.SlotSize, 4, Allocator.Persistent);
                            queryRef.HasData = true;
                            queryRef.Size = size;
                        }
                    }

                    combined = JobHandle.CombineDependencies(combined, ScheduleJobForComponent(queryRef, component, inputDeps));
                }
            }

            return combined;
        }
    }
}