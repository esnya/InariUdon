using UnityEditor;

namespace InariUdon.Sync
{
    [CustomEditor(typeof(SyncedFloat))]
    public class SyncedFloatEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var sliderProperty = serializedObject.FindProperty(nameof(SyncedFloat.slider));
            var hideSliderOptions = sliderProperty.objectReferenceValue == null;

            InariUdonEditorUtility.DrawVisibleProperties(
                serializedObject,
                shouldDraw: property => !hideSliderOptions || !IsSliderOnlyProperty(property.name),
                drawProperty: property => EditorGUILayout.PropertyField(property, true));

            serializedObject.ApplyModifiedProperties();
        }

        private static bool IsSliderOnlyProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(SyncedFloat.wholeNumbers):
                case nameof(SyncedFloat.exp):
                    return true;
                default:
                    return false;
            }
        }
    }
}
