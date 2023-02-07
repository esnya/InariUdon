using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;
using VRC.Udon;

namespace InariUdon.EditorTools
{
    public class USharpPrefabUpgradeHelper : EditorWindow
    {
        [Flags]
        public enum Filter {
            UpgradeRequired = 0x01,
            Upgraded = 0x02,
            SceneUpgraded = 0x04,
            PrefabUpgraded = 0x08,
            All = -1,
        }
        private class AssetEditingScope : IDisposable
        {
            public AssetEditingScope()
            {
                AssetDatabase.StartAssetEditing();
            }

            public void Dispose()
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private static Enum GetBehaviourVersion(UdonBehaviour udon)
        {
            return (Enum)typeof(UdonSharpEditorUtility).GetMethod("GetBehaviourVersion", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new[] { udon });
        }
        private static bool HasSceneBehaviourUpgradeFlag(UdonBehaviour udon)
        {
            return (bool)typeof(UdonSharpEditorUtility).GetMethod("HasSceneBehaviourUpgradeFlag", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new[] { udon });
        }
        private static void SetSceneBehaviourUpgraded(UdonBehaviour udon)
        {
            typeof(UdonSharpEditorUtility).GetMethod("SetSceneBehaviourUpgraded", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new[] { udon });
        }

        private static void UpgradePrefabs(IEnumerable<GameObject> prefabRootEnumerable)
        {
            typeof(UdonSharpEditorUtility).GetMethod("UpgradePrefabs", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new[] { prefabRootEnumerable });
        }

        private static void ClearBehaviourVariables(UdonBehaviour udon, bool clearPresistentVariables = false)
        {
            typeof(UdonSharpEditorUtility).GetMethod("ClearBehaviourVariables", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { udon, clearPresistentVariables });
        }

        private static IDictionary<string, object> GetPublicVariables(UdonBehaviour udon)
        {
            return udon.publicVariables.VariableSymbols.ToDictionary(symbol => symbol, symbol => udon.publicVariables.TryGetVariableValue(symbol, out var value) ? value : null);
        }

        private static bool IsUdonSharpUdonBehaviour(UdonBehaviour udon)
        {
            return udon.programSource is UdonSharpProgramAsset;
        }

        private bool MatchFilter(CacheItem item)
        {
            if ((filter & Filter.Upgraded) != 0 && item.version.Equals(2)) return true;
            if ((filter & Filter.UpgradeRequired) != 0 && !item.version.Equals(2)) return true;
            if ((filter & Filter.SceneUpgraded) != 0 && item.sceneUpgraded) return true;
            if ((filter & Filter.PrefabUpgraded) != 0 && !item.sceneUpgraded) return true;
            return false;
        }

        [MenuItem("InariUdon/U# Prefab Upgrade Helper")]
        private static void ShowWindow()
        {
            var window = GetWindow<USharpPrefabUpgradeHelper>();
            window.Show();
        }

        private struct CacheItem
        {
            public UdonBehaviour udon;
            public UdonSharpBehaviour usharp;
            public Enum version;
            public IDictionary<string, object> publicVariables;
            internal bool sceneUpgraded;
            internal bool expanded;
        }

        public Vector2 scrollPosition;
        public bool includeChildren = true;
        public Filter filter = Filter.All;
        private CacheItem[] cache;

        private void Scan()
        {
            AssetDatabase.Refresh();
            cache = Selection.gameObjects
                .SelectMany(o => includeChildren ? o.GetComponentsInChildren<UdonBehaviour>(true) : o.GetComponents<UdonBehaviour>())
                .Where(IsUdonSharpUdonBehaviour)
                .Select(udon =>
                {
                    var usharp = UdonSharpEditorUtility.GetProxyBehaviour(udon);
                    return new CacheItem()
                    {
                        udon = udon,
                        usharp = usharp,
                        version = GetBehaviourVersion(udon),
                        publicVariables = GetPublicVariables(udon),
                        sceneUpgraded = HasSceneBehaviourUpgradeFlag(udon),
                        expanded = false,
                    };
                })
                .Where(MatchFilter)
                .ToArray();
            Repaint();
        }

        private void UpgradePrefabs()
        {
            UpgradePrefabs(cache.Select(i => i.udon.gameObject)
                .Where(PrefabUtility.IsPartOfAnyPrefab)
                .Select(PrefabUtility.GetOutermostPrefabInstanceRoot)
                .Distinct()
                .Select(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot)
                .Distinct()
                .Select(AssetDatabase.LoadAssetAtPath<GameObject>));
            AssetDatabase.SaveAssets();
            Scan();
        }

        private void ClearBehaviourVariables()
        {
            using (new AssetEditingScope())
            {
                var udons = cache.Select(i => i.udon).ToArray();
                Undo.RecordObjects(udons, "Clear Behaviour Variables");
                foreach (var udon in udons)
                {
                    ClearBehaviourVariables(udon, false);
                    SetSceneBehaviourUpgraded(udon);
                    EditorUtility.SetDirty(udon);
                }
            }
            AssetDatabase.SaveAssets();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("U# Prefab Upgrade Helper");
            Selection.selectionChanged += Scan;
            Scan();
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= Scan;
        }

        private void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                includeChildren = EditorGUILayout.Toggle("Include Children", includeChildren);
                filter = (Filter)EditorGUILayout.EnumFlagsField("Filter", filter);
                if (check.changed) Scan();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Upgrade Prefabs")) UpgradePrefabs();
                if (GUILayout.Button("Clear Udon Variables")) ClearBehaviourVariables();
            }

            EditorGUILayout.Separator();

            if (cache is null) return;
            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scroll.scrollPosition;

                for (var i = 0; i < cache.Length; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        cache[i].expanded = EditorGUILayout.Foldout(cache[i].expanded, cache[i].udon.gameObject.name);
                        var item = cache[i];
                        using (new EditorGUILayout.HorizontalScope(GUILayout.Width(120)))
                        {
                            EditorGUILayout.LabelField(item.version.ToString(), GUILayout.Width(20));
                            if (item.sceneUpgraded) EditorGUILayout.LabelField("Scene Upgraded", GUILayout.Width(100));
                        }
                    }

                    if (cache[i].expanded)
                    {
                        var item = cache[i];
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.ObjectField("UdonSharpBehaviour", item.usharp, typeof(UdonSharpBehaviour), false);
                            EditorGUILayout.ObjectField("UdonBehaviour", item.udon, typeof(UdonSharpBehaviour), false);
                            foreach (var symbol in item.publicVariables.Keys)
                            {
                                EditorGUILayout.LabelField(symbol);
                                using (new EditorGUI.IndentLevelScope()) EditorGUILayout.LabelField(item.publicVariables[symbol]?.ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}
