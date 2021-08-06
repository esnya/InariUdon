#pragma warning disable IDE1006

using UdonSharp;
using UdonToolkit;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
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
        public UdonSharpBehaviour eventTarget;
        [Popup("EditorGetVariables")] public string targetVariableName = "value";
        [Popup("behaviour", "@eventTarget")] public string targetEventName = "OnValueChanged";
        public bool writeAsArray = false;

        [SectionHeader("UI Integration")]
        public Slider slider;
        public bool exp;

        [UdonSynced(UdonSyncMode.Smooth)] private float _syncValue;

        private void Start()
        {
            _syncValue = value;
            ReadSliderValue();
            if (_syncValue != value) SendEvent(value);
        }

        public override void OnPreSerialization()
        {
            _syncValue = value;
        }

        public override void OnDeserialization()
        {
            if (_syncValue != value)
            {
                if (slider != null) slider.value = exp ? Mathf.Log(_syncValue) : _syncValue;
                SendEvent(_syncValue);
            }
            value = _syncValue;
        }

        private void ReadSliderValue()
        {
            if (slider == null) return;
            value = exp ? Mathf.Exp(slider.value) : slider.value;
        }

        private void SendEvent(float value)
        {
            if (writeAsArray)
            {
                var array = (float[])eventTarget.GetProgramVariable(targetVariableName);
                if (array == null) array = new float[1];
                for (int i = 0; i < array.Length; i++) array[i] = value;
                eventTarget.SetProgramVariable(targetVariableName, array);
            }
            else eventTarget.SetProgramVariable(targetVariableName, value);
            eventTarget.SendCustomEvent(targetEventName);
        }

        public void _Sync()
        {
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            ReadSliderValue();
            if (_syncValue != value) SendEvent(value);
            _syncValue = value;
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public string[] EditorGetVariables()
        {
            this.UpdateProxy();
            if (eventTarget == null) return new []{ "" };
            return eventTarget.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => p.FieldType == typeof(float) || p.FieldType == typeof(float[]))
                .Select(p => p.Name)
                .ToArray();
        }

        [Button("Setup", true)]
        public void EditorSetup()
        {
            this.UpdateProxy();
            if (slider != null)
            {
                while (slider.onValueChanged.GetPersistentEventCount() > 0) UnityEventTools.RemovePersistentListener(slider.onValueChanged, 0);
                UnityEventTools.AddStringPersistentListener(slider.onValueChanged, UdonSharpEditorUtility.GetBackingUdonBehaviour(this).SendCustomEvent, nameof(_Sync));
            }
        }
#endif
    }
}
