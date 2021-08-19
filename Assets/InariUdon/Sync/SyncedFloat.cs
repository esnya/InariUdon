#pragma warning disable IDE1006
using UdonSharp;
using UdonToolkit;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using System.Linq;
using System.Reflection;
using UdonSharpEditor;
#endif

namespace InariUdon.Sync
{
    [
        CustomName("Synced Float"),
        HelpMessage("Provides single synced float variable with change detection."),
    ]
    public class SyncedFloat : UdonSharpBehaviour
    {
        [SectionHeader("Initial Value")]
        public float value;

        [SectionHeader("Program Variables")]
        public bool writeProgramVariables = false;
        public bool sendEvents = true;
        public bool writeAsArray = false;
        public bool programVariablesFromChildren = false;
        [HideIf("@programVariablesFromChildren")][HideIf("@!writeProgramVariables")][ListView("UdonBehaviours")] public UdonSharpBehaviour[] targets = {};
        [HideIf("@programVariablesFromChildren")][HideIf("@!writeProgramVariables")][ListView("UdonBehaviours"), Popup("programVariable", "@targets")] public string[] variableNames = {};
        [HideIf("@programVariablesFromChildren")][HideIf("@!writeProgramVariables")][ListView("UdonBehaviours"), Popup("behaviour", "@targets")] public string[] eventNames = {};
        [HideIf("@!programVariablesFromChildren")] public Transform targetsParent;
        [HideIf("@!programVariablesFromChildren")] public string variableName, eventName;

        [SectionHeader("Animators")]
        public bool writeAnimatorParameters = false;
        [HideIf("@!writeAnimatorParameters")][ListView("Animators")] public Animator[] animators = {};
        [HideIf("@!writeAnimatorParameters")][ListView("Animators"), Popup("animatorFloat", "@animators")] public string[] animatorParameterNames = {};

        [SectionHeader("UI Integration")]
        public Slider slider;
        public bool exp;

        [UdonSynced(UdonSyncMode.Smooth), FieldChangeCallback(nameof(SyncValue))] private float _syncValue;
        private float SyncValue {
            set {
                _syncValue = value;
                if (slider != null) slider.value = exp ? Mathf.Log(_syncValue) : _syncValue;

                if (writeProgramVariables) WriteProgramVariables(value);
                if (writeAnimatorParameters) WriteAnimatorParameters(value);
            }
            get => _syncValue;
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
                    else eventTarget.SetProgramVariable(targetVariableName, value);
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

                animator.SetFloat(animatorParameterNames[i], value);
            }
        }

        private void Start()
        {
            if (programVariablesFromChildren)
            {
                var targetCount = 0;
                for(int i = 0; i < targetsParent.childCount; i++)
                {
                    if ((UdonSharpBehaviour)targetsParent.GetChild(i).GetComponent(typeof(UdonBehaviour)) != null) targetCount++;
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
            if (ReadSliderValue()) SyncValue = value;
        }

        private bool ReadSliderValue()
        {
            if (slider == null) return true;
            SyncValue = exp ? Mathf.Exp(slider.value) : slider.value;
            return false;
        }

        public void _Sync()
        {
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            if (ReadSliderValue()) SyncValue = value;
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR

        // [Button("Setup", true)]
        // public void EditorSetup()
        // {
        //     this.UpdateProxy();
        //     if (slider != null)
        //     {
        //         while (slider.onValueChanged.GetPersistentEventCount() > 0) UnityEventTools.RemovePersistentListener(slider.onValueChanged, 0);
        //         UnityEventTools.AddStringPersistentListener(slider.onValueChanged, UdonSharpEditorUtility.GetBackingUdonBehaviour(this).SendCustomEvent, nameof(_Sync));
        //     }
        // }
#endif
    }
}
