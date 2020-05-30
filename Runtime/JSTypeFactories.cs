using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityReactUIElements
{
    public static class JSTypeFactories
    {
        private static readonly Dictionary<string, IJSBindingFactory> factoryCache = new Dictionary<string, IJSBindingFactory>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize()
        {
            factoryCache.Clear();

            var factoryInterface = typeof(IJSBindingFactory);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(t => t != factoryInterface && factoryInterface.IsAssignableFrom(t));

                foreach (var type in types)
                {
                    var instance = (IJSBindingFactory)Activator.CreateInstance(type);

                    if (!factoryCache.ContainsKey(instance.Name))
                    {
                        factoryCache.Add(instance.Name, instance);
                    }
                }
            }
        }

        public static IJSBindingFactory GetFactory(string name)
        {
            if (!factoryCache.ContainsKey(name)) return null;

            return factoryCache[name];
        }
    }
}