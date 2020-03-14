using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace UnityReactUIElements
{
    [AlwaysUpdateSystem]
    public abstract class ReactUIElementsQuery : JobComponentSystem
    {
        public abstract string Name { get; }

        public NativeList<Guid> queryIds;
        public NativeList<Guid> executedQueries;

        public abstract void AddComponentDataUpdateRequest(int componentIndex, int index, string data);

        protected override void OnCreate()
        {
            base.OnCreate();

            queryIds = new NativeList<Guid>(Allocator.Persistent);

            ReactUIElementsQueryRegistry.Register(this);
        }

        protected override void OnDestroy()
        {
            ReactUIElementsQueryRegistry.Remove(this);

            queryIds.Dispose();

            base.OnDestroy();
        }

        protected string ListToJson<T>(NativeArray<T> list) where T : struct, IComponentData
        {
            var stringList = new List<string>();
            var type = typeof(T);

            for (var i = 0; i < list.Length; i++)
            {
                stringList.Add(EntityFactory.ToJson(type, list[i]));
            }

            return $"[{string.Join(",", stringList)}]";
        }
    }
}