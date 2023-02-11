
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace InariUdon.Sync
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncedMultiObjectToggle : UdonSharpBehaviour
    {
        public GameObject[] targets = {};
        public bool initialState = true;
        public float initializeDelay = 10;
        [UdonSynced, FieldChangeCallback(nameof(State))] private bool _state;
        private bool State
        {
            set
            {
                _state = value;
                foreach (var target in targets)
                {
                    if (target != null) target.SetActive(value);
                }
            }
            get => _state;
        }

        private void Start()
        {
            SendCustomEventDelayedSeconds(nameof(_Initialize), initializeDelay);
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

        public void _Initialize()
        {
            State = initialState;
        }
    }
}
