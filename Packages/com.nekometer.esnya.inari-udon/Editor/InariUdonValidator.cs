using System;
using System.Reflection;
using JetBrains.Annotations;
using UdonSharp;
using UnityEditor;
using UnityEngine;

namespace InariUdon.Validation
{
    public static class InariUdonValidator
    {
        public static bool Validate<T>(T target) where T : UdonSharpBehaviour
        {
            if (Application.isPlaying) return false;
            if (Selection.activeGameObject != target.gameObject) return false;

            var type = target.GetType();
            var fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            var succeeded = true;
            foreach (var field in fields)
            {
                object GetValue() => field.GetValue(target);

                var attributes = field.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    if (attribute is NotNullAttribute)
                    {
                        succeeded = Validate(target, GetValue() != null, $"{field.Name} is required") && succeeded;
                    }
                    else if (attribute is MinAttribute min)
                    {
                        succeeded = Validate(
                            target,
                            GetValue() is int i ? i >= min.min : (float)GetValue() >= min.min,
                            $"{field.Name} must be greater than or equal {min}") && succeeded;
                    }
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
    }
}
