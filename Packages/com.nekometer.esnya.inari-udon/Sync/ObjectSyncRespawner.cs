#pragma warning disable IDE1006

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
namespace InariUdon.Sync
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ObjectSyncRespawner : UdonSharpBehaviour
    {
        public bool findObjectSyncFromChildren = false;
         public VRCObjectSync[] targets = {};
         public GameObject targetsParent;
         public bool includeDisabled;

        public override void Interact() => _Trigger();

        public void _Trigger()
        {
            if (targets == null) return;

            var localPlayer = Networking.LocalPlayer;

            if (findObjectSyncFromChildren) targets = (VRCObjectSync[])targetsParent.GetComponentsInChildren(typeof(VRCObjectSync), includeDisabled);
            foreach (var target in targets)
            {
                if (target == null) continue;
                Networking.SetOwner(localPlayer, target.gameObject);
                target.Respawn();
            }
        }
    }
}
