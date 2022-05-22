using UdonSharp;
using UdonToolkit;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace InariUdon.Sync
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncedBooleanLatch : UdonSharpBehaviour
    {
        [UdonSynced] public bool value;

        [ListView("On Set")] public UdonSharpBehaviour[] onSetEventListeners = {};
        [ListView("On Set"), Popup("behaviour", "@onSetEventListeners", true)] public string[] onSetEventNames = {};
        [ListView("On Reset")] public UdonSharpBehaviour[] onResetEventListeners = {};
        [ListView("On Reset"), Popup("behaviour", "@onResetEventListeners", true)] public string[] onResetEventNames = {};
        [ListView("On Toggle")] public UdonSharpBehaviour[] onToggleEventListeners = {};
        [ListView("On Toggle"), Popup("behaviour", "@onToggleEventListeners", true)] public string[] onToggleEventNames = {};

        private void DispatchEvent(UdonSharpBehaviour[] listeners, string[] names)
        {
            if (listeners == null || names == null) return;
            var count = Mathf.Min(listeners.Length, names.Length);
            for (int i = 0; i < count; i++)
            {
                var listener = listeners[i];
                var name = names[i];
                if (listener == null) continue;

                listener.SendCustomEvent(name);
            }
        }

        private void DispatchEvents()
        {
            if (value) DispatchEvent(onSetEventListeners, onSetEventNames);
            else DispatchEvent(onResetEventListeners, onResetEventNames);
            DispatchEvent(onToggleEventListeners, onToggleEventNames);
        }

        private void ApplyModification()
        {
            RequestSerialization();
            DispatchEvents();
        }

        private void Start()
        {
            if (Networking.IsOwner(gameObject)) DispatchEvents();
        }

        public override void OnDeserialization()
        {
            DispatchEvents();
        }

        public void _TakeOwnership()
        {
            if (Networking.IsOwner(gameObject)) return;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        public void _SetValue(bool b)
        {
            _TakeOwnership();
            value = b;
            ApplyModification();
        }

        public void _Set() => _SetValue(true);
        public void _Reset() => _SetValue(false);
        public void _Toggle() => _SetValue(!value);
    }
}
