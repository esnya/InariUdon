
using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Interfaces;

namespace InariUdon.Misc
{
    
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class PickupEventTrigger : UdonSharpBehaviour
    {
        [Header("Sync Options")]
        
        public bool networked;
        
        public NetworkEventTarget networkTarget;

        [Space]

        [Header("On Pickup")]
        public bool fireOnPickup;

        public UdonSharpBehaviour[] onPickupTargets;

        public string[] onPickupEvents;

        [Space]

        [Header("On Drop")]
        public bool fireOnDrop;

        public UdonSharpBehaviour[] onDropTargets;

        public string[] onDropEvents;

        public override void OnPickup()
        {
            BroadcastEvents(onPickupTargets, onPickupEvents);
        }

        public override void OnDrop()
        {
            BroadcastEvents(onDropTargets, onDropEvents);
        }

        void BroadcastEvents(UdonSharpBehaviour[] targets, string[] events)
        {

            var length = Mathf.Min(targets.Length, events.Length);
            for (int i = 0; i < length; i++)
            {
                if (networked) targets[i].SendCustomNetworkEvent(networkTarget, events[i]);
                else targets[i].SendCustomEvent(events[i]);
            }
        }
    }
}
