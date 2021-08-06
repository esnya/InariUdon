#pragma warning disable IDE1006

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

namespace InariUdon.Sync
{
    [
        UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync),
    ]
    public class ObjectSyncRespawner : UdonSharpBehaviour
    {
        public VRCObjectSync[] targets = {};

        public override void Interact() => _Trigger();

        public void _Trigger()
        {
            if (targets == null) return;

            var localPlayer = Networking.LocalPlayer;
            foreach (var target in targets)
            {
                Networking.SetOwner(localPlayer, target.gameObject);
                target.Respawn();
            }
        }
    }
}
