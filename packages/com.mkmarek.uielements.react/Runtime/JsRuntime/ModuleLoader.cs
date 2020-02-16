using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ChakraHost.Hosting;
using UnityEngine;

namespace UnityReactUIElements.JsRuntime
{
    internal class ModuleLoader
    {
        private static JavaScriptSourceContext _currentSourceContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

        private readonly Dictionary<string, JavaScriptModuleRecord> moduleCache = new Dictionary<string, JavaScriptModuleRecord>();
        private readonly Queue<Action> _moduleParseQueue = new Queue<Action>();
        private readonly Dictionary<string, JSFileObject> _predefinedModules = new Dictionary<string, JSFileObject>();

        public void AddPredefinedModule(string moduleName, string code)
        {
            var jsObject = ScriptableObject.CreateInstance<JSFileObject>();
            jsObject.Path = moduleName;
            jsObject.Code = code;

            _predefinedModules.Add(moduleName, jsObject);
        }

        public JavaScriptValue LoadModule(JSFileObject file, string[] modulesToReload = null)
        {
            if (modulesToReload != null)
            {
                //TODO: For now we just reload all. Later on it would be nice to reaload only the changed ones and the ones
                // referencing them

                foreach (var m in moduleCache.Keys.ToList())
                {
                    if (!_predefinedModules.ContainsKey(m)) moduleCache.Remove(m);
                }
                //foreach (var m in modulesToReload)
                //{
                //    moduleCache.Remove(m);
                //}
            }

            var rootModule = CreateModule(null, file);

            ParseModuleQueue();

            var returnValue = JavaScriptModuleRecord.RunModule(rootModule);

            ParseModuleQueue();

            return returnValue;
        }
        
        private void ParseModuleQueue()
        {
            while (_moduleParseQueue.Count > 0)
            {
                _moduleParseQueue.Dequeue().Invoke();
            }
        }

        private JavaScriptModuleRecord CreateModule(JavaScriptModuleRecord? parent, JSFileObject file)
        {
            if (moduleCache.ContainsKey(file.Path))
            {
                return moduleCache[file.Path];
            }

            var result = JavaScriptModuleRecord.Create(parent, file.Path);

            moduleCache.Add(file.Path, result);
            result.HostUrl = file.Path;

            JavaScriptModuleRecord.SetFetchModuleCallback(
                result,
                (JavaScriptModuleRecord reference, JavaScriptValue name, out JavaScriptModuleRecord record) =>
                {
                    var moduleName = name.ToString();
                    var parentPath = reference.HostUrl;

                    if (_predefinedModules.ContainsKey(moduleName))
                    {
                        record = CreateModule(reference, _predefinedModules[moduleName]);

                        return JavaScriptErrorCode.NoError;
                    }

                    moduleName = moduleName.Replace("./", "");
                    var modulePath = string.IsNullOrWhiteSpace(parentPath)
                        ? moduleName
                        : $"{Path.Combine(Path.GetDirectoryName(parentPath), moduleName)}";

                    modulePath = RemovePathToResources(modulePath);

                    var module = Resources.Load<JSFileObject>(modulePath);

                    if (module == null)
                    {
                        throw new EntryPointNotFoundException($"Module {modulePath} doesn't exist'");
                    }

                    record = CreateModule(reference, module);

                    return JavaScriptErrorCode.NoError;
                });

            JavaScriptModuleRecord.SetFetchModuleScriptCallback(result, FetchImportedModuleFromScript);
            JavaScriptModuleRecord.SetNotifyReady(result, ModuleNotifyReady);

            if (file.Path != null)
            {
                _moduleParseQueue.Enqueue(() =>
                {
                    JavaScriptModuleRecord.ParseScript(result, file.Code, _currentSourceContext++);
                    // Debug.Log($"module {file.Path} Parsed");
                });
            }

            // Debug.Log($"{file.Path} module created");
            return result;
        }

        private string RemovePathToResources(string path)
        {
            var regex = new Regex("\\\\");
            path = regex.Replace(path, "/");

            var lastIndex = path.LastIndexOf("/Resources/", StringComparison.Ordinal);

            return lastIndex == -1
                ? path
                : path.Substring(lastIndex + "/Resources/".Length);
        }

        private JavaScriptErrorCode ModuleNotifyReady(JavaScriptModuleRecord module, JavaScriptValue value)
        {
            // Debug.Log("ModuleNotifyReady");
            return JavaScriptErrorCode.NoError;
        }

        private JavaScriptErrorCode FetchImportedModuleFromScript(JavaScriptSourceContext sourceContext, JavaScriptValue source, out JavaScriptModuleRecord result)
        {
            // Debug.Log("FetchImportedModuleFromScriptDelegate start");
            result = new JavaScriptModuleRecord();
            return JavaScriptErrorCode.NoError;
        }
    }
}
