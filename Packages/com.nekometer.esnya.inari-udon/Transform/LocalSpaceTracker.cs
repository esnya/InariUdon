
using UdonSharp;
using UnityEngine;

namespace InariUdon.Transforms
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    
    public class LocalSpaceTracker : UdonSharpBehaviour
    {
        [Header("Tracked Source")]
        public Transform source;
        public Transform sourceOrigin;

        [Space]
        [Header("Tracker Transform")]
        public Transform positionTarget;
        public Transform rotationTarget;

         public string updateMode = "Update";
        bool onUpdate;

        void Start()
        {
            if (updateMode != "CustomEvent") Trigger();
            onUpdate = updateMode == "Update";
        }

        void Update()
        {
            if (onUpdate) Trigger();
        }

        public void Trigger()
        {
            if (source == null) return;

            var toLocal = sourceOrigin == null ? Matrix4x4.identity : sourceOrigin.worldToLocalMatrix;

            if (positionTarget!= null)
            {
                positionTarget.localPosition = toLocal * source.position;
            }

            if (rotationTarget != null)
            {
                rotationTarget.localRotation = toLocal.rotation * source.rotation;
            }
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public string[] GetUpdateModes()
        {
            return new string[] {
                "Update",
                "Start",
                "CustomEvent",
            };
        }

        public void UseThisAsPositionTarget() => positionTarget = transform;
        public void UseThisAsRotationTarget() => rotationTarget = transform;
#endif
    }
}
