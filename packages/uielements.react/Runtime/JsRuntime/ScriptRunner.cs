using System;
using ChakraHost.Hosting;
using UnityReactUIElements.Bridge;

namespace Assets.JsRuntime
{
    internal class ScriptRunner : IDisposable
    {
        public static object locker = new object();

        private JavaScriptRuntime runtime;
        private JavaScriptContext context;

        private static JavaScriptSourceContext currentSourceContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

        public ScriptRunner()
        {
            this.runtime = JavaScriptRuntime.Create();
            this.context = this.runtime.CreateContext();

            JavaScriptContext.Current = this.context;

            Globals.Set();
        }

        public JavaScriptValue Run(string script)
        {
            Native.ThrowIfError(Native.JsRunScript(script, currentSourceContext++, "", out var value));

            return value;
        }

        public void Dispose()
        {
            JavaScriptContext.Current = JavaScriptContext.Invalid;
            runtime.Dispose();
        }
    }
}
