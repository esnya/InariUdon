#pragma warning disable IDE1006

using UdonSharp;
using UdonToolkit;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
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

        [SectionHeader("Udon Integration")]
        [ListView("Targets")] public UdonSharpBehaviour[] targets = {};
        [ListView("Targets"), Popup("programVariable", "@targets")] public string[] variableNames = {};
        [ListView("Targets"), Popup("behaviour", "@targets")] public string[] eventNames = {};
        public bool sendEvents = true;
        public bool writeAsArray = false;

        [SectionHeader("UI Integration")]
        public Slider slider;
        public bool exp;

        [UdonSynced(UdonSyncMode.Smooth), FieldChangeCallback(nameof(SyncValue))] private float _syncValue;
        private float SyncValue {
            set {
                _syncValue = value;
                if (slider != null) slider.value = exp ? Mathf.Log(_syncValue) : _syncValue;

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
            get => _syncValue;
        }

        private void Start()
        {
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
