using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace InariUdon.Transforms
{
    [CustomEditor(typeof(ScaledMultiFollower))]
    public class ScaledMultiFollowerEditor : Editor
    {
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
                Undo.RecordObject(target, "ScaledMultiFollower Sync Now");
                follower.EditorSyncNow();
                EditorUtility.SetDirty(target);
                serializedObject.Update();
            }
        }
    }
}
