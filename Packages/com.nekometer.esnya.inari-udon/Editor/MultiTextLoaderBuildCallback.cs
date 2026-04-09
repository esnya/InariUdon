using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase.Editor.BuildPipeline;

namespace InariUdon
{
    [CustomEditor(typeof(MultiTextLoader))]
    public class MultiTextLoaderBuildCallback : Editor, IVRCSDKBuildRequestedCallback
    {
        public int callbackOrder => 0;

        private void OnEnable()
        {
            Load((MultiTextLoader)target, false);
        }

        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            LoadAllInScene(true);
            return true;
        }

        private static void LoadAllInScene(bool destroy)
        {
            foreach (var loader in Object.FindObjectsOfType<MultiTextLoader>(true))
            {
                Load(loader, destroy);
            }
        }

        private static void Load(MultiTextLoader loader, bool destroy)
        {
            if (!loader) return;

            var textAssets = AssetDatabase
                .FindAssets("t:TextAsset", new[] { loader.textPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TextAsset>)
                .Where(asset => asset != null)
                .ToDictionary(asset => asset.name);

            foreach (var tmp in loader.GetComponentsInChildren<TMP_Text>(true))
            {
                if (!textAssets.TryGetValue(tmp.gameObject.name, out var textAsset)) continue;
                tmp.text = textAsset.text;
            }

            if (destroy) Object.DestroyImmediate(loader);
        }
    }
}
