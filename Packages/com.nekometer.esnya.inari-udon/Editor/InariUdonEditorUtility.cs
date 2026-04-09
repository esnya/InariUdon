using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace InariUdon
{
    public static class InariUdonEditorUtility
    {
        private static readonly Dictionary<Type, string[]> CachedPublicEventNames = new Dictionary<Type, string[]>();
        private static readonly GUILayoutOption[] MiniButtonLayout =
        {
            GUILayout.ExpandWidth(false),
        };

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

        public static IEnumerable<SerializedProperty> EnumerateVisibleProperties(SerializedObject serializedObject)
        {
            if (serializedObject == null) yield break;

            var property = serializedObject.GetIterator();
            if (!property.NextVisible(true)) yield break;

            while (property.NextVisible(false))
            {
                yield return property.Copy();
            }
        }

        public static void DrawVisibleProperties(
            SerializedObject serializedObject,
            Func<SerializedProperty, bool> shouldDraw = null,
            Action<SerializedProperty> drawProperty = null)
        {
            foreach (var property in EnumerateVisibleProperties(serializedObject))
            {
                if (shouldDraw != null && !shouldDraw(property)) continue;
                (drawProperty ?? DrawDefaultProperty)(property);
            }
        }

        public static void MatchArraySize(SerializedProperty source, params SerializedProperty[] targets)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            foreach (var target in targets)
            {
                if (target != null) target.arraySize = source.arraySize;
            }
        }

        public static IEnumerable<SerializedProperty> EnumerateArrayElements(SerializedProperty arrayProperty)
        {
            if (arrayProperty == null || !arrayProperty.isArray) yield break;

            for (var index = 0; index < arrayProperty.arraySize; index++)
            {
                yield return arrayProperty.GetArrayElementAtIndex(index);
            }
        }

        public static void DrawArrayElements(
            SerializedProperty arrayProperty,
            Action<SerializedProperty, int> drawElement = null,
            Func<SerializedProperty, int, bool> shouldDraw = null)
        {
            if (arrayProperty == null || !arrayProperty.isArray) return;

            for (var index = 0; index < arrayProperty.arraySize; index++)
            {
                var element = arrayProperty.GetArrayElementAtIndex(index);
                if (shouldDraw != null && !shouldDraw(element, index)) continue;

                if (drawElement != null) drawElement(element, index);
                else EditorGUILayout.PropertyField(element, true);
            }
        }

        public static void RecordAndDirty(string undoName, IEnumerable<UnityEngine.Object> targets, Action action)
        {
            var undoTargets = targets?
                .Where(target => target != null)
                .Distinct()
                .ToArray() ?? Array.Empty<UnityEngine.Object>();

            if (undoTargets.Length > 0)
            {
                Undo.RecordObjects(undoTargets, undoName);
            }

            action?.Invoke();

            foreach (var undoTarget in undoTargets)
            {
                EditorUtility.SetDirty(undoTarget);
            }
        }

        public static void RecordAndDirty(string undoName, Action action, params UnityEngine.Object[] targets)
        {
            RecordAndDirty(undoName, (IEnumerable<UnityEngine.Object>)targets, action);
        }

        public static void UdonPublicEventField(SerializedProperty udonProperty, SerializedProperty valueProperty, GUIContent label)
        {
            valueProperty.stringValue = UdonPublicEventField(label, udonProperty.objectReferenceValue as Component, valueProperty.stringValue);
        }
        public static void UdonPublicEventField(SerializedProperty udonProperty, SerializedProperty valueProperty)
        {
            valueProperty.stringValue = UdonPublicEventField(new GUIContent(valueProperty.displayName), udonProperty.objectReferenceValue as Component, valueProperty.stringValue);
        }
        public static void UdonPublicEventField(Component udon, SerializedProperty property)
        {
            property.stringValue = UdonPublicEventField(new GUIContent(property.displayName), udon, property.stringValue);
        }
        public static string UdonPublicEventField(GUIContent label, Component udon, string value)
        {
            if (udon == null) return EditorGUILayout.TextField(label, value);

            var events = GetUdonPublicEventNames(udon, includeNullOption: true);
            if (events.Length <= 1) return EditorGUILayout.TextField(label, value);

            var index = Mathf.Max(Array.IndexOf(events, value), 0);
            index = EditorGUILayout.Popup(label, index, events);

            var newValue = events[index];
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

        public static T PopupField<T>(GUIContent label, T value, IEnumerable<T> values, Func<T, string> getLabel = null, string emptyLabel = null)
        {
            var options = values?.Distinct().ToList() ?? new List<T>();
            if (options.Count == 0)
            {
                return value;
            }

            getLabel ??= option => option?.ToString() ?? string.Empty;

            var displayOptions = options.Select(getLabel).ToList();
            if (emptyLabel != null)
            {
                options.Insert(0, default);
                displayOptions.Insert(0, emptyLabel);
            }

            var index = Mathf.Max(options.FindIndex(option => EqualityComparer<T>.Default.Equals(option, value)), 0);
            index = EditorGUILayout.Popup(label, index, displayOptions.ToArray());
            return options[index];
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

        public static void ObjectPopupField<T>(
            SerializedProperty property,
            IEnumerable<T> values,
            GUIContent label = null,
            string emptyLabel = null,
            Func<T, string> getLabel = null) where T : UnityEngine.Object
        {
            property.objectReferenceValue = PopupField(
                label ?? new GUIContent(property.displayName),
                property.objectReferenceValue as T,
                values,
                getLabel ?? (value => value ? value.name : string.Empty),
                emptyLabel);
        }

        public static void ObjectFieldWithSelection<T>(SerializedProperty property, GUIContent label = null, string buttonLabel = "From Selected")
            where T : UnityEngine.Object
        {
            property.objectReferenceValue = ObjectFieldWithSelection(
                label ?? new GUIContent(property.displayName),
                property.objectReferenceValue as T,
                buttonLabel);
        }

        public static T ObjectFieldWithSelection<T>(GUIContent label, T value, string buttonLabel = "From Selected")
            where T : UnityEngine.Object
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                value = EditorGUILayout.ObjectField(label, value, typeof(T), true) as T;
                if (GUILayout.Button(buttonLabel, EditorStyles.miniButton, MiniButtonLayout))
                {
                    value = GetSelectedObject<T>() ?? value;
                }
            }

            return value;
        }

        public static T GetSelectedObject<T>() where T : UnityEngine.Object
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                return Selection.activeGameObject ? Selection.activeGameObject.GetComponent<T>() : null;
            }

            return Selection.activeObject as T;
        }

        public static string[] GetUdonPublicEventNames(Component udon, bool includeNullOption = false)
        {
            if (udon == null) return includeNullOption ? new[] { "(null)" } : Array.Empty<string>();

            var behaviourType = udon.GetType();
            if (!CachedPublicEventNames.TryGetValue(behaviourType, out var events))
            {
                events = behaviourType
                    .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                    .Where(method =>
                        method.ReturnType == typeof(void) &&
                        !method.IsSpecialName &&
                        !method.ContainsGenericParameters &&
                        method.GetParameters().Length == 0)
                    .Select(method => method.Name)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToArray();
                CachedPublicEventNames[behaviourType] = events;
            }

            if (!includeNullOption) return events;

            var values = new string[events.Length + 1];
            values[0] = "(null)";
            Array.Copy(events, 0, values, 1, events.Length);
            return values;
        }

        private static void DrawDefaultProperty(SerializedProperty property)
        {
            EditorGUILayout.PropertyField(property, true);
        }
    }
}
