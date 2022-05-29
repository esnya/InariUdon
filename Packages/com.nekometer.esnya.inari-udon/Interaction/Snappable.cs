
using InariUdon.Transforms;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

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

        public override void OnPickup()
        {
            Release();
        }

        private void Snap(Transform target)
        {
            snapTarget = target;
            if (snapTarget)
            {
                transform.SetPositionAndRotation(snapTarget.position, snapTarget.rotation);
                if (reparent) transform.SetParent(snapTarget);
            }
        }

        public void Release()
        {
            snapTarget = null;
            if (reparent) transform.SetParent(initialParent);
        }

        public override void OnDrop()
        {
            Snap(FindSnapTarget());
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