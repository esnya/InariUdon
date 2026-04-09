using UnityEditor;
using UnityEngine;

namespace InariUdon.Transforms
{
    [CustomEditor(typeof(LocalSpaceTracker))]
    public class LocalSpaceTrackerEditor : Editor
    {
        private static readonly string[] UpdateModes =
        {
            "Update",
            "Start",
            "CustomEvent",
        };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var tracker = (LocalSpaceTracker)target;

            InariUdonEditorUtility.DrawVisibleProperties(
                serializedObject,
                drawProperty: property =>
                {
                    if (property.name == nameof(LocalSpaceTracker.updateMode))
                    {
                        InariUdonEditorUtility.StringPopupField(property, UpdateModes, new GUIContent(property.displayName));
                        return;
                    }

                    EditorGUILayout.PropertyField(property, true);
                });

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Use This As Position Target"))
                {
                    InariUdonEditorUtility.RecordAndDirty("Set Position Target", () => tracker.positionTarget = tracker.transform, tracker);
                }
                if (GUILayout.Button("Use This As Rotation Target"))
                {
                    InariUdonEditorUtility.RecordAndDirty("Set Rotation Target", () => tracker.rotationTarget = tracker.transform, tracker);
                }
            }
        }
    }
}
