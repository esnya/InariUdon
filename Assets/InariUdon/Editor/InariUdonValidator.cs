#if !COMPILER_UDONSHARP && UNITY_EDITOR
using JetBrains.Annotations;
using UnityEngine;
using UdonSharp;
using UdonSharpEditor;
using System.Reflection;
using System;
using UnityEditor;
#endif

namespace InariUdon.Editor
{
    public class InariUdonValidator
    {
#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public static bool Validate<T>(T target) where T : UdonSharpBehaviour
        {
            if (Application.isPlaying) return false;

            // if (Selection.activeGameObject != target.gameObject) target.UpdateProxy();
            if (Selection.activeGameObject != target.gameObject) return false;
            // var udon = UdonSharpEditorUtility.GetBackingUdonBehaviour(target);
            // if (udon == null) return;

            var type = target.GetType();
            var fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
                var succeeded = true;
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
                    if (attribute is NotNullAttribute) succeeded = Validate(target, GetValue() != null,  $"{field.Name} is required") && succeeded;
                    else if (attribute is MinAttribute min) succeeded = Validate(target, GetValue() is int i ? i >= min.min : (float)GetValue() >= min.min,  $"{field.Name} must be greater than or equal {min}") && succeeded;
                }
            }

            return succeeded;
        }

        public static bool Validate(UnityEngine.Object target, bool condition, string message)
        {
            if (Application.isPlaying) return false;
            if (!condition) Debug.LogError(message, target);
            return condition;
        }
#endif
    }
}
