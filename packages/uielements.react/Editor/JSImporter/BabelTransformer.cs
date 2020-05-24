using System;
using System.Text;
using System.Threading;
using Assets.JsRuntime;
using ChakraHost.Hosting;
using UnityEngine;

namespace UnityReactUIElements.Editor
{
    public static class BabelTransformer
    {
        public static string Transform(string code)
        {
            lock (ScriptRunner.locker)
            {
                var codeInBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(code));

                using (var runner = new ScriptRunner())
                {
                    try
                    {
                        runner.Run(JSLibraries.BabelTransform); ;

                        var retValue = runner.Run($"transpile(`{codeInBase64}`)");

                        var status = retValue.GetProperty(JavaScriptPropertyId.FromString("status"));
                        var result = retValue.GetProperty(JavaScriptPropertyId.FromString("result"));
                        var error = retValue.GetProperty(JavaScriptPropertyId.FromString("error"));

                        if (status.ToBoolean())
                        {
                            return result.ToString();
                        }

                        Debug.LogError(error.ToString());

                        return string.Empty;
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
            }

            return string.Empty;
        }
    }
}
