using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace UnityReactUIElements
{
    public static class EntityFactory
    {
        [Serializable]
        internal class CreateWithComponentsPayload {
            [Serializable]
            public class ComponentPayload
            {
                public string name;
                public string props;
            }

            public ComponentPayload[] components;
        }

        private class ComponentProperties
        {
            public Func<string, object> Deserialize { get; set; }
            public Func<object, string> Serialize { get; set; }
            public ComponentType Type { get; set; }
            public MethodInfo AddComponentMethod { get; set; }

            public void CreateComponent(Entity entity, EntityManager entityManager, string data)
            {
                var payload = ConvertFromJson(data);

                AddComponentMethod.Invoke(null, new object[] { entity, entityManager, payload });
            }

            public object ConvertFromJson(string data)
            {
                return Deserialize?.Invoke(data) ?? JsonUtility.FromJson(data, Type.GetManagedType());
            }

            public string ConvertToJson(object data)
            {
                return Serialize?.Invoke(data) ?? JsonUtility.ToJson(data);
            }
        }

       private static readonly MethodInfo AddComponentMethod = typeof(EntityFactory).GetMethod(nameof(AddComponent));

        private static readonly IDictionary<string, ComponentProperties> componentProperties = new Dictionary<string, ComponentProperties>();

        public static void RegisterComponent<TComponent>(string name, Func<string, TComponent> deserialize = null, Func<TComponent, string> serialize = null)
            where TComponent : struct, IComponentData
        {
            var type = typeof(TComponent);

            Func<string, object> deserializeFunction = null;
            if (deserialize != null)
            {
                deserializeFunction = (data) => deserialize(data);
            }

            Func<object, string> serializeFunction = null;
            if (serialize != null)
            {
                serializeFunction = (data) => serialize((TComponent)data);
            }

            componentProperties.Add(name, new ComponentProperties
            {
                Type = type,
                AddComponentMethod = AddComponentMethod.MakeGenericMethod(type),
                Deserialize = deserializeFunction,
                Serialize = serializeFunction
            });
        }

        internal static void CreateWithComponents(string data)
        {
            var payload = JsonUtility.FromJson<CreateWithComponentsPayload>(data);
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var entity = entityManager.CreateEntity();

            foreach (var component in payload.components)
            {
                if (!componentProperties.ContainsKey(component.name))
                {
                    throw new InvalidOperationException($"Component {component.name} not registered");
                }

                componentProperties[component.name].CreateComponent(entity, entityManager, component.props);
            }
        }

        internal static void RemoveEntity(int index, string[] components)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var componentTypeList = new List<ComponentType>();

            foreach (var component in components)
            {
                if (!componentProperties.ContainsKey(component))
                {
                    throw new InvalidOperationException($"Component {component} not registered");
                }

                componentTypeList.Add(componentProperties[component].Type);
            }

            var query = entityManager.CreateEntityQuery(componentTypeList.ToArray());
            var entities = query.ToEntityArray(Allocator.TempJob);

            if (index >= 0 && index < entities.Length)
            {
                entityManager.DestroyEntity(entities[index]);
            }
            else
            {
                Debug.LogError($"Invalid index {index} while removing entities. Total entity count: {entities.Length}");
            }

            entities.Dispose();
            query.Dispose();
        }

        internal static object FromJson(ComponentType component, string data)
        {
            var properties = componentProperties.Values.SingleOrDefault(e => e.Type == component);
            return properties == null
                ? JsonUtility.FromJson(data, component.GetManagedType())
                : properties.ConvertFromJson(data);
        }

        internal static string ToJson(ComponentType component, object data)
        {
            var properties = componentProperties.Values.SingleOrDefault(e => e.Type == component);
            return properties == null
                ? JsonUtility.ToJson(data)
                : properties.ConvertToJson(data);
        }

        public static void AddComponent<T>(Entity entity, EntityManager entityManager, T payload)
            where T: struct, IComponentData
        {
            entityManager.AddComponent<T>(entity);
            entityManager.SetComponentData(entity, payload);
        }
    }
}
