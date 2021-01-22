using UdonSharp;
using UdonToolkit;
using UnityEngine;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace EsnyaFactory.InariUdon
{
    [CustomName("Float Value Driver")]
    [HelpMessage("Drives float parameters of animators by one float value calculated from scene.")]
    public class FloatValueDriver : UdonSharpBehaviour
    {
        #region Public Variables
        [SectionHeader("Value Calculation Mode")]
        [Popup("GetModeOptions")][OnValueChanged("OnModeStringChanged")][UTEditor]
        public string modeString;
        [HideInInspector] public int mode;

        [Space]

        [SectionHeader("Value Sources")]
        [HideIf("HideTransformSource")][UTEditor]
        public Transform sourceTransform;
        [HideIf("HideDirectionVector")][UTEditor]
        public Vector3 localVector;
        [HideIf("HideDirectionVector")][UTEditor]
        public Vector3 worldVector;

        [Space]
        [SectionHeader("Drive Targets")][UTEditor]
        public bool driveAnimatorParameters ;

        [HideIf("@!driveAnimatorParameters")][ListView("Animator Triggers List")][UTEditor]
        public Animator[] targetAnimators;
        [ListView("Animator Triggers List")]
        [Popup("GetTargetAnimatorParameters")]
        [UTEditor]
        public string[] targetAnimatorParameters;
#endregion
        #region Unity Events
        void Update()
        {
            var value = CalculateValue();
            Drive(value);
        }
        #endregion

        #region Drivers
        void Drive(float value)
        {
            if (driveAnimatorParameters) DriveAnimatorParameters(value);
        }

        void DriveAnimatorParameters(float value)
        {
            if (targetAnimators == null || targetAnimatorParameters == null)
            {
                Debug.LogError($"[{nameof(FloatValueDriver)}({gameObject.name})] targetAnimators and targetAnimatorParameters is requried.");
                return;
            }

            var length = Mathf.Min(targetAnimators.Length, targetAnimatorParameters.Length);
            for (int i = 0; i < length; i++)
            {
                targetAnimators[i].SetFloat(targetAnimatorParameters[i], value);
            }
        }
        #endregion

        #region Calculators
        float CalculateValue()
        {
            switch (mode)
            {
                case 0:
                    return DirectionInnerProduct();
                default:
                    Debug.LogError($"[{nameof(FloatValueDriver)}({gameObject.name})] Invalid calculation mode: {mode}.");
                    return 0;
            }
        }

        float DirectionInnerProduct()
        {
            if (sourceTransform == null) {
                Debug.LogError($"[{nameof(FloatValueDriver)}({gameObject.name})] sourceTransform is requried.");
                return 0;
            }

            return Vector3.Dot(sourceTransform.rotation * localVector, worldVector);
        }
        #endregion

        #region Editor Utilities
#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public string[] GetModeOptions() => new [] {"Direction Inner Product"};

        public void OnModeStringChanged() => mode = GetModeOptions().Select((s, i) => (s, i)).FirstOrDefault(t => t.Item1 == modeString).Item2;

        public bool HideTransformSource() => mode != 0;
        public bool HideDirectionVector() => mode != 0;

        public string[] GetTargetAnimatorParameters(SerializedProperty prop)
        {
            var animator = prop.objectReferenceValue as Animator;
            if (animator == null) return new string[] {};

            return animator.parameters.Where(p => p.type == AnimatorControllerParameterType.Float).Select(p => p.name).ToArray();
        }
#endif
        #endregion
    }
}
