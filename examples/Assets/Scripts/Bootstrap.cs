using Unity.Entities;
using UnityEngine;

namespace UnityReactUIElements.Examples
{
    public sealed class Bootstrap
    {
        private class TodoItemModel
        {
            public byte[] data;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeWithScene()
        {
            var world = World.DefaultGameObjectInjectionWorld;

            for (var i = 0; i < 1; i++)
            {
                var counterEntity = world.EntityManager.CreateEntity();

                world.EntityManager.AddComponent<CounterComponent>(counterEntity);
                world.EntityManager.SetComponentData(counterEntity, new CounterComponent()
                {
                    IntTest = 42
                    //count = 0
                });
            }
        }
    }
}