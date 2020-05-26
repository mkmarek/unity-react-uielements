using System;
using System.Collections.Generic;
using System.Linq;
using ChakraHost.Hosting;
using Unity.Entities;

namespace UnityReactUIElements
{
    public class JsScheduledTasksSystem : ComponentSystem
    {
        private struct HeapKey : IComparable<HeapKey>
        {
            public HeapKey(Guid id, double value)
            {
                Id = id;
                Value = value;
            }

            public Guid Id { get; }
            public double Value { get; }

            public int CompareTo(HeapKey other)
            {
                var result = Value.CompareTo(other.Value);

                return result == 0 ? Id.CompareTo(other.Id) : result;
            }
        }

        private SortedDictionary<HeapKey, (JavaScriptValue, JavaScriptValue)> tasks = new SortedDictionary<HeapKey, (JavaScriptValue, JavaScriptValue)>();

        public void AddTask(JavaScriptValue callbackValue, JavaScriptValue callee, double delay)
        {
            var beginAt = Time.ElapsedTime + delay;
            tasks.Add(new HeapKey(Guid.NewGuid(), beginAt), (callbackValue, callee));
        }

        protected override void OnUpdate()
        {
            if (tasks.Count == 0) return;

            var min = tasks.First();

            if (min.Key.Value < Time.ElapsedTime)
            {
                min.Value.Item1.CallFunction(min.Value.Item2);

                Native.JsRelease(min.Value.Item1, out var refCount);
                Native.JsRelease(min.Value.Item2, out refCount);

                tasks.Remove(min.Key);
            }
        }
    }
}