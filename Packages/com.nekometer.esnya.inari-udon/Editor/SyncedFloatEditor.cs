using UdonSharpEditor;
using UnityEditor;

namespace InariUdon.Sync
{
    [CustomEditor(typeof(SyncedFloat))]
    public class SyncedFloatEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.Update();

            var sliderProperty = serializedObject.FindProperty(nameof(SyncedFloat.slider));
            var hideSliderOptions = sliderProperty.objectReferenceValue == null;

            var property = serializedObject.GetIterator();
            property.NextVisible(true);

            while (property.NextVisible(false))
            {
                switch (property.name)
                {
                    case nameof(SyncedFloat.wholeNumbers):
                    case nameof(SyncedFloat.exp):
                        if (hideSliderOptions) continue;
                        break;
                }
                EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
