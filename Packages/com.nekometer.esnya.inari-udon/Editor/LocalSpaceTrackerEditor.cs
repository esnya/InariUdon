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

            InariUdonEditorUtility.DrawVisibleProperties(
                serializedObject,
                drawProperty: property =>
                {
                    if (property.name == nameof(LocalSpaceTracker.updateMode))
                    {
                        InariUdonEditorUtility.StringPopupField(property, updateModes, new GUIContent(property.displayName));
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
                    InariUdonEditorUtility.RecordAndDirty("Set Position Target", () => tracker.UseThisAsPositionTarget(), tracker);
                }
                if (GUILayout.Button("Use This As Rotation Target"))
                {
                    InariUdonEditorUtility.RecordAndDirty("Set Rotation Target", () => tracker.UseThisAsRotationTarget(), tracker);
                }
            }
        }
    }
}
