#if !COMPILER_UDONSHARP && UNITY_EDITOR
using JetBrains.Annotations;
using UnityEngine;
using UdonSharp;
using UdonSharpEditor;
using System.Reflection;
using System;
using UnityEditor;

namespace InariUdon
{
    public class InariUdonValidator
    {
        public static void Validate<T>(T target) where T : UdonSharpBehaviour
        {
            if (Application.isPlaying) return;

            // if (Selection.activeGameObject != target.gameObject) target.UpdateProxy();
            if (Selection.activeGameObject != target.gameObject) return;
            // var udon = UdonSharpEditorUtility.GetBackingUdonBehaviour(target);
            // if (udon == null) return;

            var type = target.GetType();
            var fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields)
            {
                // var value = udon.GetProgramVariable(field.Name);
                object GetValue() => field.GetValue(target);
                // Debug.Log(field.Name);
                // Debug.Log(value);

                var attributes = field.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    // Debug.Log(attribute);
                    if (attribute is NotNullAttribute) Validate(target, GetValue() != null,  $"{field.Name} is required");
                    else if (attribute is MinAttribute min) Validate(target, GetValue() is int i ? i >= min.min : (float)GetValue() >= min.min,  $"{field.Name} must be greater than or equal {min}");
                }
            }
        }

        public static void Validate(UnityEngine.Object target, bool condition, string message)
        {
            if (Application.isPlaying) return;
            if (!condition) Debug.LogError(message, target);
        }
    }
}
#endif
