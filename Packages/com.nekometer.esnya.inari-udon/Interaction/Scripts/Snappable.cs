
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace InariUdon.Interaction
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(VRCPickup))]
    public class Snappable : UdonSharpBehaviour
    {
        public LayerMask layerMask = -1;
        public string[] snapTargetPrefixes = {
            "SNAP_TARGET_",
        };
        public bool reparent;

        private Transform snapTarget;
        private Transform initialParent;
        private void Start()
        {
            initialParent = transform.parent;
        }

        public override void OnPickup() => _Release();
        public override void OnDrop() => _Snap();

        public void _TakeOwnership()
        {
            if (Networking.IsOwner(gameObject)) return;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        public void _Snap()
        {
            snapTarget = FindSnapTarget();
            Debug.Log($"[{this}] Snapping");
            if (snapTarget)
            {
                _TakeOwnership();
                transform.SetPositionAndRotation(snapTarget.position, snapTarget.rotation);
                if (reparent) transform.SetParent(snapTarget);
                Debug.Log($"[{this}] Snapped {snapTarget}");
            }
        }

        public void _Release()
        {
            Debug.Log($"[{this}] Releasing");
            _Snap();
            snapTarget = null;
            if (reparent) transform.SetParent(initialParent);
            Debug.Log($"[{this}] Released");
        }

        private Transform FindSnapTarget()
        {
            foreach (var collider in Physics.OverlapSphere(transform.position, 0.001f, layerMask, QueryTriggerInteraction.Collide))
            {
                if (!collider) continue;

                foreach (var prefix in snapTargetPrefixes) {
                    if (collider.gameObject.name.StartsWith(prefix))
                    {
                        return collider.transform;
                    }
                }
            }

            return null;
        }
    }
}