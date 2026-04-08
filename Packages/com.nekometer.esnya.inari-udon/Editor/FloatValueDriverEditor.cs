using UnityEditor;
using UnityEngine;
using UdonSharpEditor;

namespace InariUdon.Driver
{
    [CustomEditor(typeof(FloatValueDriver))]
    public class FloatValueDriverEditor : Editor
    {
        private string[] _modeOptions;

        private void OnEnable()
        {
            _modeOptions = ((FloatValueDriver)target).GetModeOptions();
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            serializedObject.Update();

            var driver = (FloatValueDriver)target;
            var modeOptions = _modeOptions ?? driver.GetModeOptions();

            var modeProp = serializedObject.FindProperty(nameof(FloatValueDriver.mode));
            var modeStringProp = serializedObject.FindProperty(nameof(FloatValueDriver.modeString));

            // Keep mode in sync with modeString on load (handles legacy data)
            var currentIndex = System.Array.IndexOf(modeOptions, modeStringProp.stringValue);
            if (currentIndex < 0) currentIndex = modeProp.intValue;
            if (currentIndex < 0 || currentIndex >= modeOptions.Length) currentIndex = 0;

            // Write back immediately if modeString and mode are out of sync (e.g. migrated scenes)
            if (modeProp.intValue != currentIndex || modeStringProp.stringValue != modeOptions[currentIndex])
            {
                modeProp.intValue = currentIndex;
                modeStringProp.stringValue = modeOptions[currentIndex];
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.BeginChangeCheck();
            var newIndex = EditorGUILayout.Popup(new GUIContent("Mode"), currentIndex, modeOptions);
            if (EditorGUI.EndChangeCheck())
            {
                modeProp.intValue = newIndex;
                modeStringProp.stringValue = modeOptions[newIndex];
            }

            var property = serializedObject.GetIterator();
            property.NextVisible(true);

            while (property.NextVisible(false))
            {
                switch (property.name)
                {
                    case nameof(FloatValueDriver.modeString):
                    case nameof(FloatValueDriver.mode):
                        break;
                    case nameof(FloatValueDriver.transformOrigin):
                        if (!driver.HideTransformOrigin())
                            EditorGUILayout.PropertyField(property);
                        break;
                    case nameof(FloatValueDriver.localVector):
                        if (!driver.HideLocalVector())
                            EditorGUILayout.PropertyField(property);
                        break;
                    case nameof(FloatValueDriver.worldVector):
                        if (!driver.HideWorldVector())
                            EditorGUILayout.PropertyField(property);
                        break;
                    case nameof(FloatValueDriver.axisVector):
                        if (!driver.HideAxisVector())
                            EditorGUILayout.PropertyField(property);
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
