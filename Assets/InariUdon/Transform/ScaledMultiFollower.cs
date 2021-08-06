using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UdonSharpEditor;
#endif

namespace InariUdon.Transforms
{

    [
        CustomName("Scaled Multi Follower"),
        HelpMessage(@"
Drive multiple transform of targets by source transforms in single Update loop.
Scale of positions and origin of transforms can be changed.
This component allows you to display the position of an object on the minimap,  object placement or etc.
        "),
        Documentation.ImageAttachments("https://user-images.githubusercontent.com/2088693/121690092-5d425980-cb00-11eb-9518-a19896cbabd5.png"),
        UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync),
    ]
    public class ScaledMultiFollower : UdonSharpBehaviour
    {
        [SectionHeader("Source")]
        [Tooltip("Find source from children")] public Transform sourceParent;
        [Tooltip("Use specified sources")] [HideIf("EditorHideSources")] public Transform[] sources;
        [Tooltip("Position origin of sources")] public Transform sourceOrigin;
        [Tooltip("Find sources by path")] public bool findSourceChild;
        [Tooltip("Find sources by path")][HideIf("@!findSourceChild")] public string sourceChildPath;

        [SectionHeader("Target")]
        [Tooltip("Find targets from children")] public Transform targetParent;
        [Tooltip("Use specified targets")][HideIf("EditorHideTargets")] public Transform[] targets;
        [Tooltip("Position origin of targets")] public Transform targetOrigin;
        [Tooltip("Find targets by path")] public bool findTargetChild;
        [Tooltip("Find targets by path")] [HideIf("@!findTargetChild")] public string targetChildPath;

        [SectionHeader("Transforms")]
        [Tooltip("Scale positions")] public Vector3 positionScale = Vector3.one;
        [Tooltip("Scale positions")] public float scaleMultiplier = 1.0f;
        public bool inverseScale = false;
        [Tooltip("Enable rotation copy")] public bool rotation = true;

        [SectionHeader("Other Options")]
        public float updateFrequency = 900;
        [Tooltip("Copy `GameObject.activeSelf`")] public bool copyActive = false;
        public bool deactivateExcessiveTargets = true;
        [Tooltip("Follow if owenr of source")] public bool ownerOnly = false;
        [Tooltip("Disable collider while `pickup.IsHeld == true` of source")] public bool toggleTargetColliders = false;
        [Tooltip("Stop following while `pickup.IsHeld == true` of source")] public bool freezeTargetWhileSoruceHeld = false;

        private int count;
        private bool[] activeFlags;
        private VRCPickup[] sourcePickups;
        private Collider[] targetColliders;
        private int updateInterval, updatePerFrame, targetIndex = 0;

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

            updatePerFrame = Mathf.Min(Mathf.Max(Mathf.FloorToInt(Time.fixedUnscaledDeltaTime * updateFrequency), 1), count);
            updateInterval = Mathf.Max(Mathf.FloorToInt(1.0f / updateFrequency), 1);

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

        public void _Trigger()
        {
            var calcluatedPositionScale = inverseScale ? new Vector3(1.0f / positionScale.x, 1.0f / positionScale.y, 1.0f / positionScale.z) / scaleMultiplier : positionScale * scaleMultiplier;
            for (int i = 0; i < updatePerFrame; i++)
            {
                targetIndex = (targetIndex + 1) % count;

                var source = sources[targetIndex];
                if (!Utilities.IsValid(source) || ownerOnly && !Networking.IsOwner(source.gameObject)) continue;

                var target = targets[targetIndex];
                if (!Utilities.IsValid(target)) continue;

                if (copyActive)
                {
                    var active = source.gameObject.activeInHierarchy;
                    if (active != activeFlags[targetIndex])
                    {
                        var trailRenderer = target.GetComponentInChildren<TrailRenderer>();
                        if (trailRenderer != null) trailRenderer.Clear();

                        activeFlags[targetIndex] = active;
                        target.gameObject.SetActive(active);
                    }
                }

                var sourcePickup = sourcePickups[targetIndex];
                if (!freezeTargetWhileSoruceHeld || sourcePickup == null || !sourcePickup.IsHeld)
                {
                    var sourcePosition = source.position - (sourceOrigin == null ? Vector3.zero : sourceOrigin.position);
                    var scaledPosition = Vector3.Scale(sourcePosition, calcluatedPositionScale);
                    target.position = scaledPosition + (targetOrigin == null ? Vector3.zero : targetOrigin.position);

                    if (rotation) target.rotation = source.rotation;
                }

                if (toggleTargetColliders && targetColliders[targetIndex] != null && sourcePickup != null)
                {
                    targetColliders[targetIndex].enabled = !sourcePickup.IsHeld;
                }
            }
        }

        private void Update()
        {
            if (Time.frameCount % updateInterval != 0) return;
            _Trigger();
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public bool EditorHideSources() => sourceParent != null;
        public bool EditorHideTargets() => targetParent != null;

        [Button("Sync Now", true)]
        public void EditorSyncNow()
        {
            this.UpdateProxy();
            Start();
            updatePerFrame = count;
            _Trigger();
        }
#endif
    }
}
