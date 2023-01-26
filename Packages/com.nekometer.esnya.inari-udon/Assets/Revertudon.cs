using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using VRC.Udon;
using UdonSharp;
using System.Text.RegularExpressions;
using System;
#endif

namespace InariUdon
{
    public class Revertudon : MonoBehaviour
    {
        [Multiline] public string includePattern = "^.*$";
        [Multiline] public string excludePattern = "^VRCWorld$";

        private void Reset()
        {
            hideFlags = HideFlags.DontSaveInBuild;
        }

#if UNITY_EDITOR
        public void Revert()
        {
            var includeRegex = new Regex(String.Join("|", includePattern.Split(new [] { "\r\n", "\n", "\r" }, StringSplitOptions.None)));
            var excludeRegex = string.IsNullOrEmpty(excludePattern) ? null : new Regex(String.Join("|", excludePattern.Split(new [] { "\r\n", "\n", "\r" }, StringSplitOptions.None)));
            foreach (var udon in GetComponentsInChildren<UdonSharpBehaviour>(true).Where(PrefabUtility.IsPartOfAnyPrefab))
            {
                var name = udon.gameObject.name;
                if (!includeRegex.IsMatch(name) || (excludeRegex?.IsMatch(name) ?? false)) continue;

                Debug.Log($"[AutoRevert] Reverting {udon}", udon);
                Undo.RecordObject(udon, "Revertudon");
                PrefabUtility.RevertObjectOverride(udon, InteractionMode.AutomatedAction);
            }
        }

        public static void RevertAll(Scene scene)
        {
            foreach (var autoRevert in scene.GetRootGameObjects().SelectMany(o => o.GetComponentsInChildren<Revertudon>(true)))
            {
                autoRevert.Revert();
            }
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorSceneManager.sceneSaving += (scene, _) => RevertAll(scene);
        }
#endif
    }
}
