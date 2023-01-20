using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using VRC.Udon;
using UdonSharp;
#endif

namespace InariUdon
{

    public class AutoRevert : MonoBehaviour
    {
        private void Reset()
        {
            hideFlags = HideFlags.DontSaveInBuild;
        }

#if UNITY_EDITOR
        public void Revert()
        {
            foreach (var gameObject in GetComponentsInChildren<Transform>(true)
                .Where(PrefabUtility.IsPartOfAnyPrefab)
                .Select(t => PrefabUtility.GetOutermostPrefabInstanceRoot(t))
                .Distinct()
                .Where(r => PrefabUtility.HasPrefabInstanceAnyOverrides(r, false))
            )
            {
                Debug.Log($"[AutoRevert] Reverting {gameObject}", gameObject);
                PrefabUtility.RevertPrefabInstance(gameObject, InteractionMode.AutomatedAction);
            }

        }

        public static void RevertAll(Scene scene)
        {
            foreach (var autoRevert in scene.GetRootGameObjects().SelectMany(o => o.GetComponentsInChildren<AutoRevert>(true)))
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