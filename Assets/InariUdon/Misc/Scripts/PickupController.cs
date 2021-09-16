using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Components;
using VRC.SDKBase;

#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UdonSharpEditor;
using UnityEditor;
#endif

namespace InariUdon.Misc
{
    [
        CustomName("Pickup Controller"),
        HelpMessage("Enhancement VRC_Pickup such as relay events or expose Respawn event."),
        UdonBehaviourSyncMode(BehaviourSyncMode.None),
        RequireComponent(typeof(VRCPickup)),
    ]
    public class PickupController : UdonSharpBehaviour
    {
        #region Public Variables
        [SectionHeader("Respawn")][HelpBox("Set null to use initial world transform")]
        public Transform respawnTarget;
        public bool respawnOnDrop = false;

        [SectionHeader("Collider")] public bool overrideIsTrigger;

        [SectionHeader("Send Events")]
        public bool fireOnPickup;

        [HideIf("@!fireOnPickup")][ListView("OnPickup Target List")] public UdonSharpBehaviour[] onPickupTargets;
        [ListView("OnPickup Target List") , Popup("behaviour", "@onPickupTargets", true)] public string[] onPickupEvents;

        public bool fireOnDrop;
        [HideIf("@!fireOnDrop")] public NetworkEventTarget onDropNetworkTarget;
        [HideIf("@!fireOnDrop")][ListView("OnDrop Target List")] public UdonSharpBehaviour[] onDropTargets;
        [ListView("OnDrop Target List"), Popup("behaviour", "@onDropTargets", true)] public string[] onDropEvents;


        public bool fireOnPickupUseDown;
        [HideIf("@!fireOnPickupUseDown")][ListView("OnPickupUseDown Target List")] public UdonSharpBehaviour[] onPickupUseDownTargets;
        [ListView("OnPickupUseDown Target List"), Popup("behaviour", "@onPickupUseDownTargets", true)] public string[] onPickupUseDownEvents;
        #endregion

        Vector3 initialPosition;
        Quaternion initialRotation;
        new Collider collider;

        void Start()
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            collider = GetComponent<Collider>();

            var pickup = (VRCPickup)GetComponent(typeof(VRCPickup));
            if (fireOnPickupUseDown) pickup.AutoHold = VRC_Pickup.AutoHoldMode.Yes;
        }

        public override void OnPickup()
        {
            if (overrideIsTrigger) collider.isTrigger = true;

            if (fireOnPickup) BroadcastCustomEvent(onPickupTargets, onPickupEvents);
        }

        public override void OnDrop()
        {
            if (overrideIsTrigger) collider.isTrigger = false;

            if (fireOnDrop) BroadcastCustomEvent(onDropTargets, onDropEvents);
            if (respawnOnDrop) _Respawn();
        }

        public override void OnPickupUseDown()
        {
            if (fireOnPickupUseDown) BroadcastCustomEvent(onPickupUseDownTargets, onPickupUseDownEvents);
        }

        public void _TakeOwnership()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        public void _Respawn()
        {
            _TakeOwnership();

            var sync = (VRCObjectSync)GetComponent(typeof(VRCObjectSync));
            if (sync == null)
            {
                if (respawnTarget == null)
                {
                    transform.position = initialPosition;
                    transform.rotation = initialRotation;
                }
                else
                {
                    transform.position = respawnTarget.position;
                    transform.rotation = respawnTarget.rotation;
                }
            }
            else
            {
                if (respawnTarget == null) sync.Respawn();
                else sync.TeleportTo(respawnTarget);
            }
        }

        private void BroadcastCustomEvent(UdonSharpBehaviour[] targets, string[] events)
        {
            if (targets == null || events == null) return;

            var length = Mathf.Min(targets.Length, events.Length);
            for (var i = 0; i < length; i++) {
                targets[i].SendCustomEvent(events[i]);
            }
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        private void OnValidate()
        {
            if (fireOnPickupUseDown) {
                var pickup = GetComponent<VRCPickup>();
                Undo.RecordObject(pickup, "Enable AutoHold");
                pickup.AutoHold = VRC_Pickup.AutoHoldMode.Yes;
            }
        }
#endif
    }
}
