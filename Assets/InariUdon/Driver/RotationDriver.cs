#pragma warning disable IDE1006

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace InariUdon.Driver
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class RotationDriver : UdonSharpBehaviour
    {
        public Transform target;
        public Vector3 axis = Vector3.right;
        public bool localSpace = true, applyAngleOnStart;
        public float startAngle = 0.0f, endAngle = 90.0f, speed = 1.0f;

        private float startTime = -1;

        private void UpdateRotation(float angle)
        {
            if (target == null) return;
            var rotation = Quaternion.AngleAxis(angle, axis);
            if (localSpace) target.localRotation = rotation;
            else target.rotation = rotation;
        }

        private void Start()
        {
            if (applyAngleOnStart) UpdateRotation(startAngle);
            enabled = false;
        }

        private void Update()
        {
            if (startTime < 0) return;
            var time = Time.time - startTime;
            UpdateRotation(Mathf.Lerp(startAngle, endAngle, time * speed));
            if (time >= 1.0f)
            {
                enabled = false;
                startTime = -1;
            }
        }

        public void _Trigger()
        {
            enabled = true;
            startTime = Time.time;
        }
    }
}
