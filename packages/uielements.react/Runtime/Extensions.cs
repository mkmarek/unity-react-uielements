using System;
using System.Runtime.InteropServices;
using ChakraHost.Hosting;
using UnityEngine;

namespace UnityReactUIElements
{
    internal static class Extensions
    {
        public static JavaScriptValue ToJavaScriptValue<T>(this T obj)
            where T : class
        {
            var handle = GCHandle.Alloc(obj);
            var p = GCHandle.ToIntPtr(handle);

            return JavaScriptValue.CreateExternalObject(p, Finalizer);
        }

        public static T ObjectFromJavaScriptValue<T>(this JavaScriptValue value)
            where T: class
        {
            if (!value.HasExternalData) return default(T);

            var ptr = value.ExternalData;
            var handle = GCHandle.FromIntPtr(ptr);

            return handle.Target as T;
        }

        private static void Finalizer(IntPtr data)
        {
            var handle = GCHandle.FromIntPtr(data);
            handle.Free();
        }
    }
}