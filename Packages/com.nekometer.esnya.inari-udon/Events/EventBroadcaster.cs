using UdonSharp;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace InariUdon
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class EventBroadcaster : UdonSharpBehaviour
    {
        public GameObject parent;
        public string eventName = "_Trigger";
        public bool includeInactive;
        public bool networked;
        public NetworkEventTarget networkEventTarget;

        public void _Trigger()
        {
            if (!parent) return;
            foreach (var o in parent.GetComponentsInChildren(typeof(UdonBehaviour), includeInactive))
            {
                var udon = (UdonBehaviour)o;
                if (!udon) continue;

                if (networked) udon.SendCustomNetworkEvent(networkEventTarget, eventName);
                else udon.SendCustomEvent(eventName);
            }
        }
    }
}