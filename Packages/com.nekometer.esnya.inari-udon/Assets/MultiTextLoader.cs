
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using VRC.SDKBase.Editor.BuildPipeline;
#endif

namespace InariUdon
{
    public class MultiTextLoader : MonoBehaviour
    {
        public string textPath;

#if UNITY_EDITOR
        private void Start()
        {
            Load();
        }

        private void Load()
        {
            var textAssets = AssetDatabase
                .FindAssets("t:TextAsset", new [] { textPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TextAsset>)
                .ToDictionary(t => t.name);
            foreach (var tmp in GetComponentsInChildren<TMP_Text>(true))
            {
                var name = tmp.gameObject.name;
                if (!textAssets.ContainsKey(name)) continue;

                tmp.text = textAssets[name].text;
            }

            DestroyImmediate(this);
        }

        private static void LoadAllInScene()
        {
            foreach (var o in FindObjectsOfType(typeof(MultiTextLoader)))
            {
                (o as MultiTextLoader)?.Load();
            }
        }


        public class BuildCallback : Editor, IVRCSDKBuildRequestedCallback
        {
            public int callbackOrder => 0;

            public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
            {
                LoadAllInScene();
                return true;
            }
        }
#endif
    }
}
