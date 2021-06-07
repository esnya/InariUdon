using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Net;
using VRC.SDK3.Components;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UdonSharpEditor;
#endif

namespace EsnyaFactory.InariUdon
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ScaledMultiFollower : UdonSharpBehaviour
    {
        [SectionHeader("Source")]
        public Transform sourceParent;
        [HideIf("HideSources")] public Transform[] sources;
        public Transform sourceOrigin;
        public bool findSourceChild;
        [HideIf("@!findSourceChild")] public string sourceChildPath;

        [SectionHeader("Target")]
        public Transform targetParent;
        [HideIf("HideTargets")] public Transform[] targets;
        public Transform targetOrigin;
        public bool findTargetChild;
        [HideIf("@!findTargetChild")] public string targetChildPath;

        [SectionHeader("Transforms")]
        public Vector3 positionScale = Vector3.one;
        public float scaleMultiplier = 1.0f;
        public bool inverseScale = false;
        public bool rotation = true;

        [SectionHeader("Other Options")]
        public bool copyActive = false;
        public bool deactivateExcessiveTargets = true;
        public bool ownerOnly = false;
        public bool toggleTargetColliders = false;
        public bool freezeTargetWhileSoruceHeld = false;

        private int count;
        private bool[] activeFlags;
        private VRCPickup[] sourcePickups;
        private Collider[] targetColliders;

        private Transform[] GetChildren(Transform parent, bool find, string path)
        {
            var count = parent.childCount;
            var children = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                children[i] = parent.GetChild(i);
                if (find) children[i] = children[i].Find(path);
            }
            return children;
        }

        private void Start()
        {
            if (sourceParent != null) sources = GetChildren(sourceParent, findSourceChild, sourceChildPath);
            if (targetParent != null) targets = GetChildren(targetParent, findTargetChild, targetChildPath);

            count = Mathf.Min(sources.Length, targets.Length);
            activeFlags = new bool[count];
            sourcePickups = new VRCPickup[count];
            targetColliders = new Collider[count];

            for (int i = 0; i < count; i++)
            {
                var active = sources[i].gameObject.activeInHierarchy;
                if (copyActive) targets[i].gameObject.SetActive(active);
                activeFlags[i] = active;

                sourcePickups[i] = (VRCPickup)sources[i].GetComponent(typeof(VRCPickup));
                targetColliders[i] = targets[i].GetComponent<Collider>();
            }

            if (deactivateExcessiveTargets)
            {
                for (int i = count; i < targets.Length; i++)
                {
                    targets[i].gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            var calcluatedPositionScale = inverseScale ? new Vector3(1.0f / positionScale.x, 1.0f / positionScale.y, 1.0f / positionScale.z) / scaleMultiplier : positionScale * scaleMultiplier;
            for (int i = 0; i < count; i++)
            {
                var source = sources[i];
                if (!Utilities.IsValid(source) || ownerOnly && !Networking.IsOwner(source.gameObject)) continue;

                var target = targets[i];
                if (!Utilities.IsValid(target)) continue;

                if (copyActive)
                {
                    var active = source.gameObject.activeInHierarchy;
                    if (active != activeFlags[i])
                    {
                        activeFlags[i] = active;
                        target.gameObject.SetActive(active);
                    }
                }

                var sourcePickup = sourcePickups[i];
                if (!freezeTargetWhileSoruceHeld || sourcePickups[i] == null || !sourcePickup.IsHeld)
                {
                    var sourcePosition = source.position - (sourceOrigin == null ? Vector3.zero : sourceOrigin.position);
                    var scaledPosition = Vector3.Scale(sourcePosition, calcluatedPositionScale);
                    target.position = scaledPosition + (targetOrigin == null ? Vector3.zero : targetOrigin.position);

                    if (rotation) target.rotation = source.rotation;
                }

                if (toggleTargetColliders && targetColliders[i] != null && sourcePickup != null)
                {
                    targetColliders[i].enabled = !sourcePickup.IsHeld;
                }
            }
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public bool HideSources() => sourceParent != null;
        public bool HideTargets() => targetParent != null;

        [Button("Sources Use Global Space", true)]
        public void SourcesGlobalSpace()
        {
            this.UpdateProxy();
            sourceOrigin = null;
            this.ApplyProxyModifications();
        }
        [Button("Sources Use Local Space", true)]
        public void SourcesLocalSpace()
        {
            this.UpdateProxy();
            sourceOrigin = transform;
            this.ApplyProxyModifications();
        }
        [Button("Sources Use Parent Local Space", true)]
        public void SourcesParentLocalSpace()
        {
            this.UpdateProxy();
            sourceOrigin = targetParent;
            this.ApplyProxyModifications();
        }

        [Button("Targets Use Global Space", true)]
        public void TargetsGlobalSpace()
        {
            this.UpdateProxy();
            targetOrigin = null;
            this.ApplyProxyModifications();
        }
        [Button("Targets Use Local Space", true)]
        public void TargetsLocalSpace()
        {
            this.UpdateProxy();
            targetOrigin = transform;
            this.ApplyProxyModifications();
        }
        [Button("Targets Use Parent Local Space", true)]
        public void TargetsParentLocalSpace()
        {
            this.UpdateProxy();
            targetOrigin = targetParent;
            this.ApplyProxyModifications();
        }
#endif
    }
}
