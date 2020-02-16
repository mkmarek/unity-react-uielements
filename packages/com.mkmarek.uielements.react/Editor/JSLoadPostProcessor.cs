using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityReactUIElements.Editor
{
    public class JSLoadPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Any(e => e.EndsWith(".jsx") || e.EndsWith(".mjs")) && Application.isPlaying && Application.isEditor)
            {
                Debug.Log("Reloading JS files");

                ReactRenderer.Current.RunModule(importedAssets);
            }
        }
    }
}
