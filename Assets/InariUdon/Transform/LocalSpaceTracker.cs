
using UdonSharp;
using UdonToolkit;
using UnityEngine;

namespace EsnyaFactory.InariUdon.Transforms
{
    [CustomName("Local Space Tracker"), UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    [HelpMessage("Track source transform as local position/rotation. You can translate and scale by parent transform. Call \"Trigger\" custome event to update manually. All fields are optional.")]
    public class LocalSpaceTracker : UdonSharpBehaviour
    {
        [SectionHeader("Tracked Source")][UTEditor]
        public Transform source;
        public Transform sourceOrigin;

        [Space]
        [SectionHeader("Tracker Transform")][UTEditor]
        public Transform positionTarget;
        public Transform rotationTarget;

        [Popup("GetUpdateModes")] public string updateMode = "Update";
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
        string[] GetUpdateModes()
        {
            return new string[] {
                "Update",
                "Start",
                "CustomEvent",
            };
        }

        [Button("Use this as Position Target", true)] void UseThisAsPositionTarget() => positionTarget = transform;
        [Button("Use this as Rotation Target", true)] void UseThisAsRotationTarget() => rotationTarget = transform;
#endif
    }
}
