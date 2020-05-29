using System;
using System.Runtime.InteropServices;
using ChakraHost.Hosting;
using UnityEngine;

namespace UnityReactUIElements
{
    internal static class Extensions
    {
        public static JavaScriptValue ToJavaScriptValue<T>(this T obj)
        {
            var handle = GCHandle.Alloc(obj);
            var p = GCHandle.ToIntPtr(handle);

            return JavaScriptValue.CreateExternalObject(p, Finalizer);
        }

        public static T ObjectFromJavaScriptValue<T>(this JavaScriptValue value)
        {
            if (!value.HasExternalData) return default(T);

            var ptr = value.ExternalData;
            var handle = GCHandle.FromIntPtr(ptr);

            return (T)handle.Target;
        }

        private static void Finalizer(IntPtr data)
        {
            var handle = GCHandle.FromIntPtr(data);
            handle.Free();
        }

        public static bool ValidateWithExternalData<T>(this JavaScriptValue[] arguments, int index, string method)
            where T : class
        {
            var typeName = typeof(T).Name;

            if (arguments.Length <= index || !arguments[index].HasExternalData)
            {
                Debug.LogError($"{method} expects argument of type {typeName} on index: {index - 1}");
                return false;
            }

            var ptr = arguments[index].ExternalData;
            var handle = GCHandle.FromIntPtr(ptr);

            if ((handle.Target as T) == null)
            {
                Debug.LogError($"{method} expects argument of type {typeName} on index: {index - 1}");
                return false;
            }

            return true;
        }

        public static bool ValidateWithType(this JavaScriptValue[] arguments, int index, string method, JavaScriptValueType type)
        {
            if (arguments.Length <= index || arguments[index].ValueType != type)
            {
                Debug.LogError($"{method} expects argument of type {type} on index: {index - 1}");
                return false;
            }

            return true;
        }
    }
}