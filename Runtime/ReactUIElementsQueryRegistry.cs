using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityReactUIElements
{
    public static class ReactUIElementsQueryRegistry
    {
        private static readonly IDictionary<string, ReactUIElementsQuery> Systems = new Dictionary<string, ReactUIElementsQuery>();
        
        public static void Register(ReactUIElementsQuery system)
        {
            Systems.Add(system.Name, system);
        }

        public static void Remove(ReactUIElementsQuery system)
        {
            Systems.Remove(system.Name);
        }

        public static void RegisterComponentQuery(string queryId, string queryName)
        {
            if (!Systems.ContainsKey(queryName)) return;

            var system = Systems[queryName];

            system.queryIds.Add(Guid.Parse(queryId));
        }

        public static void RemoveComponentQuery(string queryId)
        {
            foreach (var system in Systems)
            {
                var index = system.Value.queryIds.IndexOf(Guid.Parse(queryId));

                if (index >= 0)
                {
                    system.Value.queryIds.RemoveAtSwapBack(index);
                }

                index = system.Value.executedQueries.IndexOf(Guid.Parse(queryId));

                if (index >= 0)
                {
                    system.Value.executedQueries.RemoveAtSwapBack(index);
                }
            }
        }

        public static void UpdateComponentData(string queryId, int componentIndex, int index, string data)
        {
            foreach (var system in Systems)
            {
                var queryIdIndex = system.Value.queryIds.IndexOf(Guid.Parse(queryId));

                if (queryIdIndex >= 0)
                {
                    system.Value.AddComponentDataUpdateRequest(componentIndex, index, data);
                }
            }
        }
    }
}