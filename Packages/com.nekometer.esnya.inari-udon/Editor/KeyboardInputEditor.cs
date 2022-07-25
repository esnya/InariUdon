using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
using System;

namespace InariUdon.Interaction
{
    [CustomEditor(typeof(KeyboardInput))]
    class KeyboardInputEditor : Editor
    {
        private readonly string[] Modes = {
            "KeyDown",
            "KeyUp",
            "KeyHold",
        };

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.Update();
            var keyCodes = serializedObject.FindProperty(nameof(KeyboardInput.keyCodes));
            var modes = serializedObject.FindProperty(nameof(KeyboardInput.modes));
            var eventTargets = serializedObject.FindProperty(nameof(KeyboardInput.eventTargets));
            var onKeyDownEvents = serializedObject.FindProperty(nameof(KeyboardInput.onKeyDownEvents));

            modes.arraySize = keyCodes.arraySize;
            eventTargets.arraySize = keyCodes.arraySize;
            onKeyDownEvents.arraySize = keyCodes.arraySize;

            EditorGUILayout.PropertyField(keyCodes, new GUIContent("Key Mapping"), false);

            if (keyCodes.isExpanded)
            {
                for (var i = 0; i < keyCodes.arraySize; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var keyCode = keyCodes.GetArrayElementAtIndex(i);
                        var mode = modes.GetArrayElementAtIndex(i);

                        keyCode.intValue = (int)(KeyCode)EditorGUILayout.EnumPopup((Enum)Enum.ToObject(typeof(KeyCode), keyCode.intValue));
                        mode.intValue = EditorGUILayout.Popup(mode.intValue, Modes);
                        EditorGUILayout.PropertyField(eventTargets.GetArrayElementAtIndex(i), GUIContent.none);
                        InariUdonEditorUtility.UdonPublicEventField(eventTargets.GetArrayElementAtIndex(i), onKeyDownEvents.GetArrayElementAtIndex(i), GUIContent.none);

                        if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                        {
                            keyCodes.DeleteArrayElementAtIndex(i);
                            modes.DeleteArrayElementAtIndex(i);
                            eventTargets.DeleteArrayElementAtIndex(i);
                            onKeyDownEvents.DeleteArrayElementAtIndex(i);
                        }
                    }
                }

                if (GUILayout.Button("Add Element"))
                {
                    keyCodes.arraySize++;
                    eventTargets.arraySize = keyCodes.arraySize;
                    onKeyDownEvents.arraySize = keyCodes.arraySize;
                }
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(KeyboardInput.holdTime)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(KeyboardInput.holdInterval)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(KeyboardInput.audioSource)));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
