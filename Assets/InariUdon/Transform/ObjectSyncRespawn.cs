using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace EsnyaFactory.InariUdon.Transforms
{
    [
        CustomName("ObjectSync Respawn"),
        HelpMessage("Simple event relay component to call `VRCObjecySync.Respawn()`"),
        UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync),
        RequireComponent(typeof(VRCObjectSync)),
    ]
    class ObjectSyncRespawn : UdonSharpBehaviour
    {
        public void Respawn()
        {
            var sync = (VRCObjectSync)GetComponent(typeof(VRCObjectSync));
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            sync.Respawn();
        }
    }
}
