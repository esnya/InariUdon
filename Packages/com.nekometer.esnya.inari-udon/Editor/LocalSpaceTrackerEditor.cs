using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace InariUdon.Transforms
{
    [CustomEditor(typeof(LocalSpaceTracker))]
    public class LocalSpaceTrackerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.Update();

            var tracker = (LocalSpaceTracker)target;
            var updateModes = tracker.GetUpdateModes();

            var property = serializedObject.GetIterator();
            property.NextVisible(true);

            while (property.NextVisible(false))
            {
                if (property.name == nameof(LocalSpaceTracker.updateMode))
                {
                    var currentIndex = System.Array.IndexOf(updateModes, property.stringValue);
                    if (currentIndex < 0) currentIndex = 0;
                    EditorGUI.BeginChangeCheck();
                    var newIndex = EditorGUILayout.Popup(new GUIContent(property.displayName), currentIndex, updateModes);
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.stringValue = updateModes[newIndex];
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(property, true);
                }
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Use This As Position Target"))
                {
                    Undo.RecordObject(tracker, "Set Position Target");
                    tracker.UseThisAsPositionTarget();
                    EditorUtility.SetDirty(tracker);
                }
                if (GUILayout.Button("Use This As Rotation Target"))
                {
                    Undo.RecordObject(tracker, "Set Rotation Target");
                    tracker.UseThisAsRotationTarget();
                    EditorUtility.SetDirty(tracker);
                }
            }
        }
    }
}
