
using UdonSharp;
using VRC.SDKBase;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UdonSharpEditor;
#endif

namespace EsnyaFactory.InariUdon.Sync
{
    public class SyncedFloat : UdonSharpBehaviour
    {
        public float value;
        public UdonSharpBehaviour eventTarget;
        public string targetVariableName = "value";
        public string targetEventName = "OnValueChanged";

        [UdonSynced(UdonSyncMode.Smooth)] private float _syncValue;

        public override void OnPreSerialization()
        {
            _syncValue = value;
        }

        public override void OnDeserialization()
        {
            if (_syncValue != value) SendEvent(_syncValue);
            value = _syncValue;
        }

        private void SendEvent(float value)
        {
            if (!Utilities.IsValid(eventTarget)) return;

            eventTarget.SetProgramVariable(targetVariableName, value);
            eventTarget.SendCustomEvent(targetEventName);
        }

        public void Sync()
        {
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            if (_syncValue != value) SendEvent(value);
            _syncValue = value;
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        private string[] GetVariables()
        {
            this.UpdateProxy();
            if (eventTarget == null) return new []{ "" };
            return eventTarget.GetType().GetProperties(BindingFlags.Public).Where(p => p.PropertyType == typeof(float)).Select(p => p.Name).ToArray();
        }
#endif
    }
}
