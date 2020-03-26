using UnityEditor;

namespace UnityReactUIElements.Editor
{
    using UnityEngine;
    using UnityEditor.Experimental.AssetImporters;
    using System.IO;

    [ScriptedImporter(1, "jsx")]
    public class JsxFileImporter : ScriptedImporter
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
    public class MjsFileImporter : JsxFileImporter
    {
    }

    [CustomEditor(typeof(JSFileObject))]
    public class JSFileObjectInspector : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            var obj = (JSFileObject)target;

            EditorGUILayout.TextArea(obj.Code);
        }
    }
}
