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

            var networked = serializedObject.FindProperty(nameof(TouchSwitch.networked)).boolValue;
            var knobMode = serializedObject.FindProperty(nameof(TouchSwitch.knobMode)).boolValue;
            var wheelMode = serializedObject.FindProperty(nameof(TouchSwitch.wheelMode)).boolValue;
            var directionalMode = serializedObject.FindProperty(nameof(TouchSwitch.directionalMode)).boolValue;

            InariUdonEditorUtility.DrawVisibleProperties(
                serializedObject,
                shouldDraw: property => ShouldDrawProperty(property.name, networked, knobMode, wheelMode, directionalMode),
                drawProperty: property =>
                {
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
                            EditorGUILayout.PropertyField(property, true);
                            break;
                    }
                });

            serializedObject.ApplyModifiedProperties();
        }

        private static bool ShouldDrawProperty(string propertyName, bool networked, bool knobMode, bool wheelMode, bool directionalMode)
        {
            switch (propertyName)
            {
                case nameof(TouchSwitch.networkEventTarget):
                    return networked;
                case nameof(TouchSwitch.onKnobRight):
                case nameof(TouchSwitch.onKnobLeft):
                case nameof(TouchSwitch.knobStep):
                    return knobMode;
                case nameof(TouchSwitch.onWheelLeft):
                case nameof(TouchSwitch.onWheelRight):
                case nameof(TouchSwitch.wheelStep):
                    return wheelMode;
                case nameof(TouchSwitch.directionalThreshold):
                case nameof(TouchSwitch.onUp):
                case nameof(TouchSwitch.onDown):
                case nameof(TouchSwitch.onLeft):
                case nameof(TouchSwitch.onRight):
                    return directionalMode;
                default:
                    return true;
            }
        }
    }
}
