#pragma warning disable IDE0051,IDE1006,IDE0018

using UdonSharp;
using UnityEngine;
using UdonToolkit;
using VRC.SDKBase;

namespace InariUdon.Trigger
{

    [
        HelpMessage("Trigger by Enable/Disable Component and GameObject"),
        UdonBehaviourSyncMode(BehaviourSyncMode.None),
    ]
    public class EnabledTrigger : UdonSharpBehaviour
    {
        [ListView("Enabled Events")] public UdonSharpBehaviour[] enabledEventTargets = {};
        [ListView("Enabled Events"), Popup("behaviour", "@enabledEventTargets", true)] public string[] enabledEvents = {};
        [ListView("Disabled Events")] public UdonSharpBehaviour[] disabledEventTargets = {};
        [ListView("Disabled Events"), Popup("behaviour", "@disabledEventTargets", true)] public string[] disabledEvents = {};
        public bool ownerOnly = false;

        private void SendCustomEventToTargets(UdonSharpBehaviour[] targets, string[] eventNames)
        {
            if (targets == null || eventNames == null) return;
            var length = Mathf.Min(targets.Length, eventNames.Length);
            for (int i = 0; i < length; i++)
            {
                var target = targets[i];
                if (target == null || ownerOnly && !Networking.IsOwner(gameObject)) continue;
                targets[i].SendCustomEvent(eventNames[i]);
            }
        }

        private void OnEnable()
        {
            SendCustomEventToTargets(enabledEventTargets, enabledEvents);
        }

        private void OnDisable()
        {
            SendCustomEventToTargets(disabledEventTargets, disabledEvents);
        }
    }
}
