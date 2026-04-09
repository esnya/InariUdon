using System.Collections.Generic;
using System.Linq;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace InariUdon.Transforms
{
    [CustomEditor(typeof(ScaledMultiFollower))]
    public class ScaledMultiFollowerEditor : Editor
    {
        private static Transform[] GetChildren(Transform parent, bool findChild, string childPath)
        {
            if (parent == null) return null;

            var children = new Transform[parent.childCount];
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                children[i] = findChild ? child.Find(childPath) : child;
            }

            return children;
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.Update();

            var follower = (ScaledMultiFollower)target;

            var property = serializedObject.GetIterator();
            property.NextVisible(true);

            while (property.NextVisible(false))
            {
                switch (property.name)
                {
                    case nameof(ScaledMultiFollower.sources):
                        if (follower.EditorHideSources()) continue;
                        break;
                    case nameof(ScaledMultiFollower.targets):
                        if (follower.EditorHideTargets()) continue;
                        break;
                }
                EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            if (GUILayout.Button("Sync Now"))
            {
                var undoTargets = new List<Object> { target };

                var resolvedTargets = follower.targetParent != null
                    ? GetChildren(follower.targetParent, follower.findTargetChild, follower.targetChildPath)
                    : follower.targets;

                if (resolvedTargets != null)
                {
                    foreach (var syncedTarget in resolvedTargets.Where(syncedTarget => syncedTarget != null))
                    {
                        undoTargets.Add(syncedTarget);
                        undoTargets.Add(syncedTarget.gameObject);
                    }
                }

                Undo.RecordObjects(undoTargets.ToArray(), "ScaledMultiFollower Sync Now");
                follower.EditorSyncNow();
                EditorUtility.SetDirty(target);
                serializedObject.Update();
            }
        }
    }
}
