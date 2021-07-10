
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace InariUdon.Driver
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class SyncedSpinner : UdonSharpBehaviour
    {
        [UdonSynced(UdonSyncMode.Linear)] public float angle = 0.0f;
        public Vector3 axis = Vector3.up;
        public float speed = 1.0f;
        public bool randomizeStartAngle = true;

        private void Start()
        {
            axis = axis.normalized;
            if (Networking.IsOwner(gameObject)) angle = Random.Range(0, 360) % 360;
        }

        private void Update()
        {
            if (Networking.IsOwner(gameObject)) angle += speed * Time.deltaTime % 360.0f;
            transform.localRotation = Quaternion.AngleAxis(angle, axis);
        }
    }
}
