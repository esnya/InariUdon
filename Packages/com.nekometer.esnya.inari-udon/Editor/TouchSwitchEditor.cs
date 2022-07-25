using UnityEngine;
using UnityEditor;
using UdonSharpEditor;

namespace InariUdon.Interaction
{
    [CustomEditor(typeof(TouchSwitch))]
    public class TouchSwitchEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.Update();

            var property = serializedObject.GetIterator();
            property.NextVisible(true);

            var networked = serializedObject.FindProperty(nameof(TouchSwitch.networked)).boolValue;
            var knobMode = serializedObject.FindProperty(nameof(TouchSwitch.knobMode)).boolValue;
            var wheelMode = serializedObject.FindProperty(nameof(TouchSwitch.wheelMode)).boolValue;
            var directionalMode = serializedObject.FindProperty(nameof(TouchSwitch.directionalMode)).boolValue;

            while (property.NextVisible(false))
            {
                switch (property.name)
                {
                    case nameof(TouchSwitch.networkEventTarget):
                        if (!networked) continue;
                        break;
                    case nameof(TouchSwitch.onKnobRight):
                    case nameof(TouchSwitch.onKnobLeft):
                    case nameof(TouchSwitch.knobStep):
                        if (!knobMode) continue;
                        break;
                    case nameof(TouchSwitch.onWheelLeft):
                    case nameof(TouchSwitch.onWheelRight):
                    case nameof(TouchSwitch.wheelStep):
                        if (!wheelMode) continue;
                        break;
                    case nameof(TouchSwitch.directionalThreshold):
                    case nameof(TouchSwitch.onUp):
                    case nameof(TouchSwitch.onDown):
                    case nameof(TouchSwitch.onLeft):
                    case nameof(TouchSwitch.onRight):
                        if (!directionalMode) continue;
                        break;
                }

                switch (property.name)
                {
                    case nameof(TouchSwitch.eventName):
                    case nameof(TouchSwitch.onKnobRight):
                    case nameof(TouchSwitch.onKnobLeft):
                    case nameof(TouchSwitch.onWheelLeft):
                    case nameof(TouchSwitch.onWheelRight):
                    case nameof(TouchSwitch.onUp):
                    case nameof(TouchSwitch.onDown):
                    case nameof(TouchSwitch.onLeft):
                    case nameof(TouchSwitch.onRight):
                        InariUdonEditorUtility.UdonPublicEventField(serializedObject.FindProperty(nameof(TouchSwitch.eventTarget)), property);
                        break;
                    default:
                        EditorGUILayout.PropertyField(property);
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
