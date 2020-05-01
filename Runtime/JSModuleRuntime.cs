using System;
using ChakraHost.Hosting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityReactUIElements.Bridge;
using UnityReactUIElements.JsRuntime;

namespace UnityReactUIElements
{
    public class JsModuleRuntime: IDisposable
    {
        private JavaScriptRuntime runtime;
        private JavaScriptContext context;
        private JavaScriptContext.Scope scope;
        private ModuleLoader loader;

        public JsModuleRuntime()
        {
            this.runtime = JavaScriptRuntime.Create();
            this.context = runtime.CreateContext();
            this.scope = new JavaScriptContext.Scope(context);

            this.loader = new ModuleLoader();

            loader.AddPredefinedModule("react", JSLibraries.React);
            loader.AddPredefinedModule("unity-renderer", JSLibraries.UnityRenderer);
        }

        public void CheckForError()
        {
            Native.JsHasException(out var hasException);

            if (!hasException) return;

            try
            {
                Native.ThrowIfError(Native.JsGetAndClearException(out var errorObject));

                errorObject.PrintJavaScriptError();
            }
            catch (JavaScriptScriptException e)
            {
                e.Error.PrintJavaScriptError();
            }
            catch (JavaScriptFatalException e)
            {
                Debug.LogError(e);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        // Start is called before the first frame update
        public void RunModule(JSFileObject root, string[] modulesToReload = null)
        {
            Assert.IsNotNull(root, "Root can't be null'");
            Assert.IsFalse(string.IsNullOrWhiteSpace(root.Code), "Code inside root js file can't be null or empty'");
            Assert.IsFalse(string.IsNullOrWhiteSpace(root.Path), "Root path is null. Try to reimport the root file");

            try
            {
                Globals.Set();

                loader.LoadModule(root, modulesToReload);
            }
            catch (JavaScriptScriptException e)
            {
                e.Error.PrintJavaScriptError();
            }
            catch (JavaScriptFatalException e)
            {
                Debug.LogError(e);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void Dispose()
        {
            scope.Dispose();
            runtime.Dispose();
        }
    }
}