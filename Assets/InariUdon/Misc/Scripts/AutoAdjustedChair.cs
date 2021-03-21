
using System.Runtime.Remoting.Services;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace EsnyaFactory.InariUdon
{
    public class AutoAdjustedChair : UdonSharpBehaviour
    {
        public Transform seatTopFront;
        [UdonSynced(UdonSyncMode.Smooth)] private Vector2 offset;
        private VRCStation station;
        private Vector3 initialEnterPosition;
        private Transform GetEnterTransform()
        {
            return station.stationEnterPlayerLocation ?? station.transform;
        }
        private void ApplyOffset()
        {
            var enter = GetEnterTransform();
            enter.position = station.transform.TransformPoint(initialEnterPosition) + enter.forward * offset.x + enter.up * offset.y;
        }

        private void Start()
        {
            station = (VRCStation)GetComponent(typeof(VRCStation));
            initialEnterPosition = station.transform.InverseTransformPoint(GetEnterTransform().position);
        }

        bool adjust;
        private void LateUpdate() {
            if (!adjust || !Networking.IsOwner(gameObject)) return;

            var upperLeg = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftUpperLeg);
            var lowerLeg = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftLowerLeg);
            var foot = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftFoot);
            var upperLegLength = Vector3.Distance(upperLeg, lowerLeg);
            var lowerLegLength = Vector3.Distance(lowerLeg, foot);

            offset.x = Vector3.Dot(station.transform.forward, seatTopFront.position - station.transform.TransformPoint(initialEnterPosition)) - upperLegLength;
            offset.y = Vector3.Dot(station.transform.up, seatTopFront.position - station.transform.TransformPoint(initialEnterPosition)) - lowerLegLength;
            Debug.Log($"Adjusting... {offset}");
            ApplyOffset();
            adjust = false;
        }

        public override void Interact()
        {
            Networking.LocalPlayer.UseAttachedStation();
        }


        private Vector2 prevOffset;
        public override void OnDeserialization()
        {
            if (offset != prevOffset) ApplyOffset();
            prevOffset = offset;
        }

        public override void OnStationEntered(VRCPlayerApi player)
        {
            if (!player.isLocal) return;

            Networking.SetOwner(player, gameObject);
            offset = Vector2.zero;
            adjust = true;
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            if (!player.isLocal) return;
            offset = Vector2.zero;
            ApplyOffset();
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            var player = Networking.LocalPlayer;
            if (player == null) return;

            Gizmos.DrawWireSphere(player.GetBonePosition(HumanBodyBones.Hips), 0.05f);
            Gizmos.DrawWireSphere(player.GetBonePosition(HumanBodyBones.LeftUpperLeg), 0.05f);
            Gizmos.DrawWireSphere(player.GetBonePosition(HumanBodyBones.LeftLowerLeg), 0.05f);
        }
#endif
    }
}
