
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace InariUdon.Sync
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncedObjectToggle : UdonSharpBehaviour
    {
        public GameObject target;
        [UdonSynced, FieldChangeCallback(nameof(State))] private bool _state;
        private bool State
        {
            set
            {
                _state = value;
                target.SetActive(value);
            }
            get => target.activeSelf;
        }

        public override void Interact()
        {
            _ToggleActive();
        }

        public void _SetActive(bool value)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            State = value;
            RequestSerialization();
        }
        public void _Activate() => _SetActive(true);
        public void _Deactivate() => _SetActive(false);
        public void _ToggleActive() => _SetActive(!State);
    }
}
