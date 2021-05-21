
using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace EsnyaFactory.InariUdon
{
    [CustomName("Pickup Event Trigger")]
    [HelpMessage("SendCustomEvents on pickup events.")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class PickupEventTrigger : UdonSharpBehaviour
    {
        [SectionHeader("Sync Options")]
        [UTEditor]
        public bool networked;
        [HideIf("@!networked")][UTEditor]
        public NetworkEventTarget networkTarget;

        [Space]

        [SectionHeader("On Pickup")][UTEditor]
        public bool fireOnPickup;

        [ListView("OnPickup List")][HideIf("@!fireOnPickup")][UTEditor]
        public UdonSharpBehaviour[] onPickupTargets;

        [ListView("OnPickup List")][Popup("behaviour", "@onPickupTargets", true)][UTEditor]
        public string[] onPickupEvents;

        [Space]

        [SectionHeader("On Drop")][UTEditor]
        public bool fireOnDrop;

        [ListView("OnDrop List")][HideIf("@!fireOnDrop")][UTEditor]
        public UdonSharpBehaviour[] onDropTargets;

        [ListView("OnDrop List")][Popup("behaviour", "@onDropTargets", true)][UTEditor]
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
