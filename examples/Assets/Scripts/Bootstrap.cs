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

            Debug.Log(typeof(CounterComponent).Assembly.Location);

            for (var i = 0; i < 1; i++)
            {
                var counterEntity = world.EntityManager.CreateEntity();

                world.EntityManager.AddComponent<CounterComponent>(counterEntity);
                world.EntityManager.SetComponentData(counterEntity, new CounterComponent()
                {
                    //count = 0
                });
            }

            EntityFactory.RegisterComponent<CounterComponent>(nameof(CounterComponent));
            EntityFactory.RegisterComponent(nameof(TodoItemComponent), (data) =>
            {
                var model = JsonUtility.FromJson<TodoItemModel>(data);
                var component = new TodoItemComponent();

                if (model.data != null)
                {
                    unsafe
                    {
                        var dataPtr = component.data;

                        for (var i = 0; i < model.data.Length && i < 128; i++)
                        {
                            *(dataPtr++) = model.data[i];
                        }
                    }
                }

                return component;
            }, component =>
            {
                var model = new TodoItemModel {data = new byte[128]};

                if (model.data != null)
                {
                    unsafe
                    {
                        var dataPtr = component.data;

                        for (var i = 0; i < model.data.Length && i < 128; i++)
                        {
                            model.data[i] = *(dataPtr++);
                        }
                    }
                }

                return JsonUtility.ToJson(model);
            });
        }
    }

    public class CounterQuerySystem : ReactUIElementsQuery<CounterComponent>
    {
        public override string Name => "Counter";
    }

    public class TodoItemsSystem : ReactUIElementsQuery<TodoItemComponent>
    {
        public override string Name => "TodoItems";
    }
}