using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace InariUdon
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(VRCPickup))]
    public class GripTransporeter : UdonSharpBehaviour
    {
        [Tooltip("Set null to use Transform.parent")] public Transform origin;
        public bool jumpOnDisable;
        private VRCPickup pickup;
        private Vector3 prevOriginPosition;
        private Vector3 relativePosition;

        private void Start()
        {
            if (!origin) origin = transform.parent;
            pickup = (VRCPickup)GetComponent(typeof(VRCPickup));

            relativePosition = origin.InverseTransformPoint(transform.position);
        }

        private void OnDisable()
        {
            if (pickup.IsHeld)
            {
                if (jumpOnDisable) Jump();
                pickup.Drop();
                ResetPosition();
            }
        }

        public override void OnPickup()
        {
            prevOriginPosition = origin.position;
        }

        public override void OnDrop()
        {
            ResetPosition();
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            if (value && pickup.IsHeld) Jump();
        }

        private void LateUpdate()
        {
            if (!pickup.IsHeld) return;

            var deltaTime = Time.deltaTime;
            var originPosition = origin.position;
            var originVelocity = (originPosition - prevOriginPosition) / deltaTime;
            prevOriginPosition = originPosition;

            var targetPosition = origin.TransformPoint(relativePosition);

            Networking.LocalPlayer.SetVelocity((targetPosition - transform.position) / deltaTime + originVelocity);
        }

        private void Jump()
        {
            pickup.Drop();
            var player = Networking.LocalPlayer;
            player.SetVelocity(player.GetVelocity() + Vector3.up * player.GetJumpImpulse());
        }

        private void ResetPosition()
        {
            transform.position = origin.TransformPoint(relativePosition);
        }
    }
}
