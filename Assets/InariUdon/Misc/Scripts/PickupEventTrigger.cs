
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
    public class PickupEventTrigger : UdonSharpBehaviour
    {
        [SectionHeader("Sync Options")]

        public bool networked;
        [HideIf("@!networked")]
        public NetworkEventTarget networkTarget;

        [Space]

        [SectionHeader("On Pickup")]
        public bool fireOnPickup;

        [ListView("OnPickup List")][HideIf("@!fireOnPickup")]
        public UdonSharpBehaviour[] onPickupTargets;

        [ListView("OnPickup List")][Popup("behaviour", "@onPickupTargets", true)]
        public string[] onPickupEvents;

        [Space]

        [SectionHeader("On Drop")]
        public bool fireOnDrop;

        [ListView("OnDrop List")][HideIf("@!fireOnDrop")]
        public UdonSharpBehaviour[] onDropTargets;

        [ListView("OnDrop List")][Popup("behaviour", "@onDropTargets", true)]
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
