using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System.Net;
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
        public bool rotation = true;

        [SectionHeader("Filters")]
        public bool ownerOnly;

        private int count;

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
        }

        private void Update()
        {
            for (int i = 0; i < count; i++)
            {
                var source = sources[i];
                if (!Utilities.IsValid(source) || ownerOnly && !Networking.IsOwner(source.gameObject)) continue;

                var target = targets[i];
                if (!Utilities.IsValid(target)) continue;

                var sourcePosition = source.position - (sourceOrigin == null ? Vector3.zero : sourceOrigin.position);
                var scaledPosition = Vector3.Scale(sourcePosition, positionScale) * scaleMultiplier;
                target.position = scaledPosition + (targetOrigin == null ? Vector3.zero : targetOrigin.position);

                if (rotation) target.rotation = source.rotation;
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
