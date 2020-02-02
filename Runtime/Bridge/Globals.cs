using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using ChakraHost.Hosting;
using UnityEngine;

namespace UnityReactUIElements.Bridge
{
    public static class Globals
    {
        private static readonly IDictionary<int, Timer> timers = new Dictionary<int, Timer>();

        public static void Set()
        {
            Native.JsGetGlobalObject(out var globalObject);
            globalObject.SetProperty(JavaScriptPropertyId.FromString("global"), globalObject, true);

            SetNativesObject(globalObject);
            SetConsoleLogObject(globalObject);
            SetTimeoutFunctions(globalObject);
            SetBase64Functions(globalObject);
        }

        public static JavaScriptValue GetInvokeCallbackFunction()
        {
            Native.JsGetGlobalObject(out var globalObject);
            Native.JsGetProperty(globalObject, JavaScriptPropertyId.FromString("natives"), out var natives);
            Native.JsGetProperty(natives, JavaScriptPropertyId.FromString("invokeCallback"), out var invokeCallback);

            return invokeCallback;
        }

        private static void SetBase64Functions(JavaScriptValue globalObject)
        {
            Native.JsCreateFunction(FromBase64, IntPtr.Zero, out var fromBase64Function);
            globalObject.SetProperty(JavaScriptPropertyId.FromString("fromBase64"), fromBase64Function, true);

            Native.JsCreateFunction(ToBase64, IntPtr.Zero, out var toBase64Function);
            globalObject.SetProperty(JavaScriptPropertyId.FromString("toBase64"), toBase64Function, true);
        }

        private static JavaScriptValue FromBase64(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            if (argumentcount < 2)
                return JavaScriptValue.FromString("");

            var base64String = arguments[1].ToString();

            return JavaScriptValue.FromString(Encoding.UTF8.GetString(Convert.FromBase64String(base64String)));
        }

        private static JavaScriptValue ToBase64(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            if (argumentcount < 2)
                return JavaScriptValue.FromString("");

            var stringToEncode = arguments[1].ToString();

            return JavaScriptValue.FromString(Convert.ToBase64String(Encoding.UTF8.GetBytes(stringToEncode)));
        }

        private static void SetTimeoutFunctions(JavaScriptValue globalObject)
        {
            Native.JsCreateFunction(SetTimeout, IntPtr.Zero, out var setTimeoutFunction);
            globalObject.SetProperty(JavaScriptPropertyId.FromString("setTimeout"), setTimeoutFunction, true);

            Native.JsCreateFunction(ClearTimeout, IntPtr.Zero, out var clearTimeoutFunction);
            globalObject.SetProperty(JavaScriptPropertyId.FromString("clearTimeout"), clearTimeoutFunction, true);
        }

        private static JavaScriptValue ClearTimeout(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            Native.JsGetUndefinedValue(out var undefinedValue);
            Native.JsNumberToInt(arguments[1], out var handle);

            if (!timers.ContainsKey(handle)) return undefinedValue;

            var timer = timers[handle];
            timers.Remove(handle);

            timer.Stop();
            timer.Dispose();

            return undefinedValue;
        }

        private static JavaScriptValue SetTimeout(JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            var interval = 0;

            if (argumentcount > 2)
                Native.JsNumberToInt(arguments[2], out interval);

            var id = Guid.NewGuid().GetHashCode();
            Native.JsIntToNumber(id, out var handleValue);

            if (interval == 0)
            {
                Native.JsCallFunction(arguments[1], new JavaScriptValue[] { }, 0, out var result);
                return handleValue;
            };

            var timer = new Timer {Interval = interval};

            timer.Elapsed += (sender, e) =>
            {
                Native.JsCallFunction(arguments[1], new JavaScriptValue[] { }, 0, out var result);
                timers.Remove(id);
                timer.Dispose();
            };

            timers.Add(id, timer);


            return handleValue;
        }

        private static void SetNativesObject(JavaScriptValue globalObject)
        {
            Native.JsCreateObject(out var nativesObject);
            Native.JsCreateFunction(Bridge, IntPtr.Zero, out var functionValue);

            nativesObject.SetProperty(JavaScriptPropertyId.FromString("bridge"), functionValue, true);
            globalObject.SetProperty(JavaScriptPropertyId.FromString("natives"), nativesObject, true);
        }

        private static void SetConsoleLogObject(JavaScriptValue globalObject)
        {
            Native.JsCreateObject(out var consoleObject);
            Native.JsCreateFunction(ConsoleLog, IntPtr.Zero, out var functionValue);

            consoleObject.SetProperty(JavaScriptPropertyId.FromString("log"), functionValue, true);
            consoleObject.SetProperty(JavaScriptPropertyId.FromString("error"), functionValue, true);
            consoleObject.SetProperty(JavaScriptPropertyId.FromString("warn"), functionValue, true);
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

        private static JavaScriptValue Bridge(
            JavaScriptValue callee, bool isconstructcall, JavaScriptValue[] arguments, ushort argumentcount, IntPtr callbackdata)
        {
            Native.JsGetUndefinedValue(out var undefinedValue);

            if (argumentcount <= 0) return undefinedValue;

            Native.JsConvertValueToString(arguments[1], out var stringValue);
            Native.JsStringToPointer(stringValue, out var resultPtr, out _);

            var resultString = Marshal.PtrToStringUni(resultPtr);

            ReactRenderer.Current.messagesToHandle.Enqueue(resultString);

            //Debug.Log(resultString);

            return undefinedValue;
        }
    }
}
