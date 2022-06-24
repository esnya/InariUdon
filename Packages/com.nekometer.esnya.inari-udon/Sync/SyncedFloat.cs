#pragma warning disable IDE1006
using UdonSharp;
using UdonToolkit;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using JetBrains.Annotations;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
#endif

namespace InariUdon.Sync
{
    /// <summary>
    /// Provides single synced float variable with several integration.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    [DefaultExecutionOrder(1000)]
    public class SyncedFloat : UdonSharpBehaviour
    {

        /// <summary>
        /// Initial value
        /// </summary>
        [Header("Initialize")] public float value;

        /// <summary>
        /// Value as integer. Casted before updating program variables.
        /// </summary>
        [Header("Cast")] public bool castToInt;

        /// <summary>
        /// Clamp value to range.
        /// </summary>
        [Header("Clamp")] public bool clampValue;

        /// <summary>
        /// min value of clamp range.
        /// </summary>
        [HideIf("@!clampValue")] public float minValue = 0.0f;

        /// <summary>
        /// max value of clamp range.
        /// </summary>
        [HideIf("@!clampValue")] public float maxValue = 1.0f;

        [Header("Increment")]
        /// <summary>
        /// value step for _Increment or _Decrement event
        /// </summary>
        public float incrementStep = 0.1f;

        [Header("Program Variables")]
        /// <summary>
        /// Integrate with other UdonBehaviours.
        /// </summary>
        public bool writeProgramVariables = false;

        /// <summary>
        /// Send event on change.
        /// </summary>
        [HideIf("@!writeProgramVariables")] public bool sendEvents = true;

        /// <summary>
        /// Write value as float[] or int[].
        /// </summary>
        [HideIf("@!writeProgramVariables")] public bool writeAsArray = false;

        /// <summary>
        /// Find targets by parent.
        /// </summary>
        [HideIf("@!writeProgramVariables")] public bool programVariablesFromChildren = false;

        /// <summary>
        /// List of target uson behaviours.
        /// </summary>
        [NotNull][HideIf("@programVariablesFromChildren")][HideIf("@!writeProgramVariables")][ListView("UdonBehaviours")] public UdonSharpBehaviour[] targets = { };

        /// <summary>
        /// List of variable names.
        /// </summary>
        [NotNull][HideIf("@programVariablesFromChildren")][HideIf("@!writeProgramVariables")][ListView("UdonBehaviours")][Popup("programVariable", "@targets")] public string[] variableNames = { };

        /// <summary>
        /// List of event names sent on cahange.
        /// </summary>
        /// <returns></returns>
        [NotNull][HideIf("@programVariablesFromChildren")][HideIf("@!writeProgramVariables")][ListView("UdonBehaviours")][Popup("behaviour", "@targets")] public string[] eventNames = { };

        /// <summary>
        /// Parent transform of targets.
        /// </summary>
        [HideIf("@!programVariablesFromChildren")] public Transform targetsParent;

        /// <summary>
        /// Common variable name and event name for all targets.
        /// </summary>
        [HideIf("@!programVariablesFromChildren")] public string variableName, eventName;

        [Header("Animators")]
        /// <summary>
        /// Integrate with animators.
        /// </summary>
        public bool writeAnimatorParameters = false;

        /// <summary>
        /// List of target animators.
        /// </summary>
        [NotNull][HideIf("@!writeAnimatorParameters")][ListView("Animators")] public Animator[] animators = { };

        /// <summary>
        /// List of animator float parameter names.
        /// </summary>
        [NotNull][HideIf("@!writeAnimatorParameters")][ListView("Animators")][Popup("animatorFloat", "@animators")] public string[] animatorParameterNames = { };

        [Header("UI")]
        /// <summary>
        /// Slider to edit value.
        ///
        /// Invoke _Sync custom event on change callback.
        /// </summary>
        public Slider slider;

        /// <summary>
        /// Force enable whole numbers.
        /// </summary>
        [HideIf("HideSliderOptions")] public bool wholeNumbers;

        /// <summary>
        /// Apply exp curve.
        /// </summary>
        [HideIf("HideSliderOptions")] public bool exp;

        [UdonSynced(UdonSyncMode.Smooth)][FieldChangeCallback(nameof(Value))] private float _value;
        private float Value
        {
            set
            {
                _value = clampValue ? Mathf.Clamp(value, minValue, maxValue) : value;
                if (slider) slider.value = exp ? Mathf.Log(_value) : _value;

                if (writeProgramVariables) WriteProgramVariables(value);
                if (writeAnimatorParameters) WriteAnimatorParameters(value);
            }
            get => _value;
        }

        private void WriteProgramVariables(float value)
        {
            var variableLength = Mathf.Min(targets.Length, variableNames.Length);
            for (int i = 0; i < variableLength; i++)
            {
                var eventTarget = targets[i];
                if (eventTarget == null) continue;
                var targetVariableName = variableNames[i];
                if (writeAsArray)
                {
                    var array = (float[])eventTarget.GetProgramVariable(targetVariableName);
                    if (array == null) array = new float[1];
                    for (int j = 0; j < array.Length; j++) array[j] = value;
                    eventTarget.SetProgramVariable(targetVariableName, array);
                }
                else
                {
                    if (castToInt) eventTarget.SetProgramVariable(targetVariableName, (int)value);
                    else eventTarget.SetProgramVariable(targetVariableName, value);
                }
            }

            if (sendEvents)
            {
                var eventLength = Mathf.Min(variableLength, eventNames.Length);
                for (int i = 0; i < eventLength; i++)
                {
                    var eventTarget = targets[i];
                    if (eventTarget == null) continue;
                    eventTarget.SendCustomEvent(eventNames[i]);
                }
            }
        }

        private void WriteAnimatorParameters(float value)
        {
            var length = Mathf.Min(animators.Length, animatorParameterNames.Length);
            for (int i = 0; i < length; i++)
            {
                var animator = animators[i];
                if (animator == null) continue;

                if (castToInt) animator.SetInteger(animatorParameterNames[i], (int)value);
                else animator.SetFloat(animatorParameterNames[i], value);
            }
        }

        private void Start()
        {
            if (programVariablesFromChildren)
            {
                var targetCount = 0;
                for (int i = 0; i < targetsParent.childCount; i++)
                {
                    if ((UdonBehaviour)targetsParent.GetChild(i).GetComponent(typeof(UdonBehaviour)) != null) targetCount++;
                }

                targets = new UdonSharpBehaviour[targetCount];
                variableNames = new string[targetCount];
                eventNames = new string[targetCount];
                for (int i = 0, j = 0; i < targetsParent.childCount && j < targetCount; i++)
                {
                    var udon = (UdonSharpBehaviour)targetsParent.GetChild(i).GetComponent(typeof(UdonBehaviour));
                    if (udon == null) continue;

                    targets[j] = udon;
                    variableNames[j] = variableName;
                    eventNames[j] = eventName;

                    j++;
                }
            }

            if (slider != null)
            {
                slider.minValue = minValue;
                slider.maxValue = maxValue;
                slider.wholeNumbers = castToInt || wholeNumbers;
            }

            Value = value;
        }

        private bool ReadSliderValue()
        {
            if (slider == null) return true;
            Value = exp ? Mathf.Exp(slider.value) : slider.value;
            return false;
        }

        /// <summary>
        /// Take ownership if not owner
        /// </summary>
        [PublicAPI]
        public void _TakeOwnership()
        {
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        /// <summary>
        /// Read slider value.
        /// </summary>
        [PublicAPI]
        public void _Sync()
        {
            _TakeOwnership();
            if (ReadSliderValue()) Value = value;
        }

        /// <summary>
        /// Increment value by incrementStep.
        /// </summary>
        [PublicAPI]
        public void _Increment()
        {
            _TakeOwnership();
            Value += incrementStep;
        }

        /// <summary>
        /// Decrement value by incrementStep.
        /// </summary>
        [PublicAPI]
        public void _Decrement()
        {
            _TakeOwnership();
            Value += incrementStep;
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public bool HideSliderOptions() => !slider;
#endif
    }
}
