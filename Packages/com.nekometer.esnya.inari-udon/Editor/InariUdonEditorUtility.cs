using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UdonSharp;
using UnityEditor;
using UnityEngine;

namespace InariUdon
{
    public class InariUdonEditorUtility
    {
        public static readonly string[] CommonVrAxes = new[]
        {
            "Oculus_CrossPlatform_PrimaryIndexTrigger",
            "Oculus_CrossPlatform_SecondaryIndexTrigger",
            "Oculus_CrossPlatform_PrimaryHandTrigger",
            "Oculus_CrossPlatform_SecondaryHandTrigger",
            "Horizontal",
            "Oculus_CrossPlatform_SecondaryThumbstickHorizontal",
            "Vertical",
            "Oculus_CrossPlatform_SecondaryThumbstickVertical",
        };

        public static readonly string[] CommonVrButtons = new[]
        {
            "Oculus_CrossPlatform_PrimaryThumbstick",
            "Oculus_CrossPlatform_SecondaryThumbstick",
            "Oculus_CrossPlatform_Button4",
            "Oculus_CrossPlatform_Button2",
        };

        public static void UdonPublicEventField(SerializedProperty udonProperty, SerializedProperty valueProperty, GUIContent label)
        {
            valueProperty.stringValue = UdonPublicEventField(label, udonProperty.objectReferenceValue as UdonSharpBehaviour, valueProperty.stringValue);
        }
        public static void UdonPublicEventField(SerializedProperty udonProperty, SerializedProperty valueProperty)
        {
            valueProperty.stringValue = UdonPublicEventField(new GUIContent(valueProperty.displayName), udonProperty.objectReferenceValue as UdonSharpBehaviour, valueProperty.stringValue);
        }
        public static void UdonPublicEventField(UdonSharpBehaviour udon, SerializedProperty property)
        {
            property.stringValue = UdonPublicEventField(new GUIContent(property.displayName), udon, property.stringValue);
        }
        public static string UdonPublicEventField(GUIContent label, UdonSharpBehaviour udon, string value)
        {
            if (udon == null) return EditorGUILayout.TextField(label, value);

            var events = udon.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).Select(m => m.Name).Prepend("(null)").ToList();
            if (events.Count == 0) return EditorGUILayout.TextField(label, value);

            var index = Mathf.Max(events.FindIndex(e => e == value), 0);
            index = EditorGUILayout.Popup(label, index, events.ToArray());

            var newValue = events.Skip(index).FirstOrDefault();
            return newValue == "(null)" ? null : newValue;
        }

        public static void StringPopupField(SerializedProperty property, IEnumerable<string> values, GUIContent label = null, string emptyLabel = null)
        {
            property.stringValue = StringPopupField(label ?? new GUIContent(property.displayName), property.stringValue, values, emptyLabel);
        }

        public static string StringPopupField(GUIContent label, string value, IEnumerable<string> values, string emptyLabel = null)
        {
            var options = values?.Where(v => !string.IsNullOrEmpty(v)).Distinct().ToList() ?? new List<string>();
            if (options.Count == 0)
            {
                return EditorGUILayout.TextField(label, value);
            }

            if (emptyLabel != null)
            {
                options.Insert(0, emptyLabel);
            }

            var currentValue = string.IsNullOrEmpty(value) && emptyLabel != null ? emptyLabel : value;
            var index = Mathf.Max(options.FindIndex(v => v == currentValue), 0);
            index = EditorGUILayout.Popup(label, index, options.ToArray());

            var newValue = options[index];
            return newValue == emptyLabel ? null : newValue;
        }

        public static void IntPopupField(SerializedProperty property, IReadOnlyList<string> values, GUIContent label = null)
        {
            property.intValue = IntPopupField(label ?? new GUIContent(property.displayName), property.intValue, values);
        }

        public static int IntPopupField(GUIContent label, int value, IReadOnlyList<string> values)
        {
            if (values == null || values.Count == 0)
            {
                return EditorGUILayout.IntField(label, value);
            }

            var index = Mathf.Clamp(value, 0, values.Count - 1);
            return EditorGUILayout.Popup(label, index, values.ToArray());
        }
    }
}
