namespace EsnyaFactory
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;
    using VRC.Udon.Common.Interfaces;

    public class PhysicalSwitch : UdonSharpBehaviour
    {
        [Header("Initial State")]
        public bool state;
        public bool syncState;
        
        [Space]
        [Header("Event Target")]
        public UdonBehaviour target;
        public string variableName = "state";
        public string eventName = "OnValueChanged";

        [Space]
        [Header("Components")]
        public Animator animator;
        public string stateName = "State";
        public AudioSource audioSource;
        public bool syncAudio = true;

        bool initialized;

        void Start()
        {
            OnValueChanged();

            initialized = true;

            if (syncState && !Networking.IsOwner(gameObject)) 
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(RequestSync));
            }
        }

        void ApplyLocalState()
        {
            if (animator != null)
            {
                animator.SetBool(stateName, state);
            }

            if (target != null)
            {
                target.SetProgramVariable(variableName, state);
                target.SendCustomEvent(eventName);
            }

            if (syncAudio) {
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PlayAudio));
            } else {
                PlayAudio();
            }
        }
        
        void Sync()
        {
            if (state)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Activate));
            }
            else
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Deactivate));
            }
        }

        public void OnValueChanged()
        {
            ApplyLocalState();

            if (syncState)
            {
                Sync();
            }
        }

        public void PlayAudio()
        {
            if (audioSource == null) return;
            audioSource.Play();
        }

        public void RequestSync()
        {
            if (!Networking.IsOwner(gameObject)) return;
            Sync();
        }

        public void Activate()
        {
            state = true;
            ApplyLocalState();
        }

        public void Deactivate()
        {
            state = false;
            ApplyLocalState();
        }

        public void Toggle()
        {
            state = !state;
            OnValueChanged();
        }
    }
}
