using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ChakraHost.Hosting;
using Unity.Entities;
using Unity.UIElements.Runtime;
using UnityEngine;

[assembly: InternalsVisibleTo("ReactUIElements.JsImporter")]
[assembly: InternalsVisibleTo("Unity.ReactUIElements.CodeGen")]

namespace UnityReactUIElements.Bridge
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class GlobalFunctionAttribute : Attribute
    {
        public GlobalFunctionAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    internal static class Globals
    {
        public static void Set(PanelRenderer renderer = null)
        {
            Native.JsGetGlobalObject(out var globalObject);
            globalObject.SetProperty(JavaScriptPropertyId.FromString("global"), globalObject, true);

            SetConsoleLogObject(globalObject);

            if (renderer != null)
            {
                globalObject.SetProperty(
                    JavaScriptPropertyId.FromString("__HOSTCONFIG__"), 
                    HostConfig.Create(),
                    true);

                globalObject.SetProperty(
                    JavaScriptPropertyId.FromString("__CONTAINER__"),
                    renderer.visualTree.ToJavaScriptValue(),
                    true);
            }

            var customFunctions = AppDomain.CurrentDomain.GetAssemblies()
                .Where(e => e.GetName().Name == "ReactUIElements" ||
                            e.GetReferencedAssemblies().Any(a => a.Name == "ReactUIElements"))
                .SelectMany(e => e.DefinedTypes)
                .SelectMany(e => e.GetMethods())
                .Where(e => e.GetCustomAttribute<GlobalFunctionAttribute>() != null)
                .Select(e => new
                {
                    attribute = e.GetCustomAttribute<GlobalFunctionAttribute>(),
                    method = e
                })
                .ToList();

            foreach (var customFunction in customFunctions)
            {
                if (!ValidateFunctionMethod(customFunction.method))
                {
                    Debug.Log("Custom global method must have the following signature static JavaScriptValue Function(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)");
                    continue;
                }

                var function = JavaScriptValue.CreateFunction(customFunction.attribute.Name, (JavaScriptNativeFunction)customFunction.method.CreateDelegate(typeof(JavaScriptNativeFunction)));

                globalObject.SetProperty(
                    JavaScriptPropertyId.FromString(customFunction.attribute.Name),
                    function,
                    true);
            }
        }

        private static bool ValidateFunctionMethod(MethodInfo method)
        {
            if (method.ReturnParameter.ParameterType != typeof(JavaScriptValue)) return false;
            if (method.GetParameters().ElementAt(0).ParameterType != typeof(JavaScriptValue)) return false;
            if (method.GetParameters().ElementAt(1).ParameterType != typeof(bool)) return false;
            if (method.GetParameters().ElementAt(2).ParameterType != typeof(JavaScriptValue[])) return false;
            if (method.GetParameters().ElementAt(3).ParameterType != typeof(ushort)) return false;
            if (method.GetParameters().ElementAt(4).ParameterType != typeof(IntPtr)) return false;

            return true;
        }

        [GlobalFunction("__CREATEQUERY__")]
        public static JavaScriptValue CreateQuery(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            if (arguments.Length < 3 ||
                arguments[1].ValueType != JavaScriptValueType.Array ||
                arguments[2].ValueType != JavaScriptValueType.Function) return JavaScriptValue.Undefined;

            var length = arguments[1].GetProperty(JavaScriptPropertyId.FromString("length")).ToInt32();

            var components = new List<string>();
            for (var i = 0; i < length; i++)
            {
                if (arguments[1].HasIndexedProperty(JavaScriptValue.FromInt32(i)))
                {
                    var prop = arguments[1].GetIndexedProperty(JavaScriptValue.FromInt32(i));

                    if (prop.ValueType != JavaScriptValueType.String) continue;

                    components.Add(prop.ToString());
                }
            }

            var querySystemType = typeof(ReactComponentQuerySystem);

            var systems = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(e => e.GetTypes())
                .Where(e => querySystemType.IsAssignableFrom(e) && e != querySystemType)
                .ToList();

            if (systems.Count > 1)
            {
                Debug.LogError("There can be only one definition of JSBindingSystem");
                return JavaScriptValue.Undefined;
            }

            if (systems.Count == 0)
            {
                Debug.LogWarning("There is no definition JSBindingSystem. Queries towards ECS won't work'");
                return JavaScriptValue.Undefined;
            }

            var system = systems.Single();

            var querySystem = (ReactComponentQuerySystem)World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(system);

            return querySystem.CreateQuery(components.ToArray(), arguments[2]);
        }

        [GlobalFunction("getFactory")]
        public static JavaScriptValue GetFactory(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            var name = arguments[1].ToString();
            var factory = JSTypeFactories.GetFactory(name);

            if (factory == null) return JavaScriptValue.Null;

            return factory.CreateConstructor();
        }

        [GlobalFunction("fromBase64")]
        public static JavaScriptValue FromBase64(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            if (argumentcount < 2)
                return JavaScriptValue.FromString("");

            var base64String = arguments[1].ToString();

            return JavaScriptValue.FromString(Encoding.UTF8.GetString(Convert.FromBase64String(base64String)));
        }

        [GlobalFunction("toBase64")]
        public static JavaScriptValue ToBase64(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            if (argumentcount < 2)
                return JavaScriptValue.FromString("");

            var stringToEncode = arguments[1].ToString();

            return JavaScriptValue.FromString(Convert.ToBase64String(Encoding.UTF8.GetBytes(stringToEncode)));
        }

        [GlobalFunction("clearTimeout")]
        public static JavaScriptValue ClearTimeout(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            Native.JsGetUndefinedValue(out var undefinedValue);

            //TODO: implement clearing

            return undefinedValue;
        }

        [GlobalFunction("setTimeout")]
        public static JavaScriptValue SetTimeout(JavaScriptValue callee, bool isConstructCall, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] JavaScriptValue[] arguments, ushort argumentCount, IntPtr callbackData)
        {
            var callbackValue = arguments[1];
            var afterValue = arguments[2].ConvertToNumber();
            var after = Math.Max(afterValue.ToDouble(), 1);

            var system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<JsScheduledTasksSystem>();
            system.AddTask(callbackValue, callee, (float)after / 1000);

            return JavaScriptValue.True;
        }

        private static void SetConsoleLogObject(JavaScriptValue globalObject)
        {
            Native.JsCreateObject(out var consoleObject);
            Native.JsCreateFunction(ConsoleLog, IntPtr.Zero, out var logFunctionValue);
            Native.JsCreateFunction(ConsoleError, IntPtr.Zero, out var errorFunctionValue);
            Native.JsCreateFunction(ConsoleWarn, IntPtr.Zero, out var warningFunctionValue);

            consoleObject.SetProperty(JavaScriptPropertyId.FromString("log"), logFunctionValue, true);
            consoleObject.SetProperty(JavaScriptPropertyId.FromString("error"), errorFunctionValue, true);
            consoleObject.SetProperty(JavaScriptPropertyId.FromString("warn"), warningFunctionValue, true);
            consoleObject.SetProperty(JavaScriptPropertyId.FromString("info"), logFunctionValue, true);
            consoleObject.SetProperty(JavaScriptPropertyId.FromString("trace"), logFunctionValue, true);
            globalObject.SetProperty(JavaScriptPropertyId.FromString("console"), consoleObject, true);
        }

        private static JavaScriptValue ConsoleLog(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            Native.JsGetUndefinedValue(out var undefinedValue);

            for (var i = 1; i < argumentcount; i++)
            {
                Native.JsConvertValueToString(arguments[i], out var stringValue);
                Native.JsStringToPointer(stringValue, out var resultPtr, out _);

                var resultString = Marshal.PtrToStringUni(resultPtr);

                Debug.Log(resultString);
            }

            return undefinedValue;
        }

        private static JavaScriptValue ConsoleError(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            Native.JsGetUndefinedValue(out var undefinedValue);

            for (var i = 1; i < argumentcount; i++)
            {
                Native.JsConvertValueToString(arguments[i], out var stringValue);
                Native.JsStringToPointer(stringValue, out var resultPtr, out _);

                var resultString = Marshal.PtrToStringUni(resultPtr);

                Debug.LogError(resultString);
            }

            return undefinedValue;
        }

        private static JavaScriptValue ConsoleWarn(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            Native.JsGetUndefinedValue(out var undefinedValue);

            for (var i = 1; i < argumentcount; i++)
            {
                Native.JsConvertValueToString(arguments[i], out var stringValue);
                Native.JsStringToPointer(stringValue, out var resultPtr, out _);

                var resultString = Marshal.PtrToStringUni(resultPtr);

                Debug.LogWarning(resultString);
            }

            return undefinedValue;
        }
    }
}
