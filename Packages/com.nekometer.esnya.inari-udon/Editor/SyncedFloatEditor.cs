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

            var syncedFloat = (SyncedFloat)target;

            var property = serializedObject.GetIterator();
            property.NextVisible(true);

            while (property.NextVisible(false))
            {
                switch (property.name)
                {
                    case nameof(SyncedFloat.wholeNumbers):
                    case nameof(SyncedFloat.exp):
                        if (syncedFloat.HideSliderOptions()) continue;
                        break;
                }
                EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
