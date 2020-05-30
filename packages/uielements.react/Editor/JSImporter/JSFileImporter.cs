using UnityEditor;

namespace UnityReactUIElements.Editor
{
    using UnityEngine;
    using UnityEditor.Experimental.AssetImporters;
    using System.IO;

    [ScriptedImporter(1, "jsx")]
    internal class JsxFileImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var code = File.ReadAllText(ctx.assetPath);
            var obj = ScriptableObject.CreateInstance<JSFileObject>();
            obj.Code = BabelTransformer.Transform(code);
            obj.Path = ctx.assetPath;

            ctx.AddObjectToAsset("main", obj);
            ctx.SetMainObject(obj);
        }
    }

    [ScriptedImporter(1, "mjs")]
    internal class MjsFileImporter : JsxFileImporter
    {
    }

    [CustomEditor(typeof(JSFileObject))]
    internal class JSFileObjectInspector : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            var obj = (JSFileObject)target;

            EditorGUILayout.TextArea(obj.Code);
        }
    }
}
