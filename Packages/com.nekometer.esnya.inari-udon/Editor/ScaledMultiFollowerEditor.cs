using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace InariUdon.Transforms
{
    [CustomEditor(typeof(ScaledMultiFollower))]
    public class ScaledMultiFollowerEditor : Editor
    {
        private static Transform[] ResolveChildren(Transform parent, bool findChild, string childPath)
        {
            if (parent == null) return null;

            var children = new Transform[parent.childCount];
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                children[i] = findChild ? child.Find(childPath) : child;
            }

            return children;
        }

        private static void SyncNow(ScaledMultiFollower follower)
        {
            if (follower.sourceParent != null) follower.sources = ResolveChildren(follower.sourceParent, follower.findSourceChild, follower.sourceChildPath);
            if (follower.targetParent != null) follower.targets = ResolveChildren(follower.targetParent, follower.findTargetChild, follower.targetChildPath);

            if (follower.sources == null || follower.targets == null) return;

            var count = Mathf.Min(follower.sources.Length, follower.targets.Length);
            if (count <= 0) return;

            var scale = follower.inverseScale
                ? new Vector3(1.0f / follower.positionScale.x, 1.0f / follower.positionScale.y, 1.0f / follower.positionScale.z) / follower.scaleMultiplier
                : follower.positionScale * follower.scaleMultiplier;

            for (var i = 0; i < count; i++)
            {
                var source = follower.sources[i];
                var target = follower.targets[i];
                if (source == null || target == null) continue;

                var sourcePosition = source.position - (follower.sourceOrigin == null ? Vector3.zero : follower.sourceOrigin.position);
                var scaledPosition = Vector3.Scale(sourcePosition, scale);
                target.position = scaledPosition + (follower.targetOrigin == null ? Vector3.zero : follower.targetOrigin.position);

                if (follower.rotation) target.rotation = source.rotation;
            }
        }

        private static Transform[] GetChildren(Transform parent, bool findChild, string childPath)
        {
            if (parent == null) return null;

            var children = new Transform[parent.childCount];
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                children[i] = findChild ? child.Find(childPath) : child;
            }

            return children;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var follower = (ScaledMultiFollower)target;

            InariUdonEditorUtility.DrawVisibleProperties(
                serializedObject,
                shouldDraw: property => ShouldDrawProperty(property.name, follower),
                drawProperty: property => EditorGUILayout.PropertyField(property, true));

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            if (GUILayout.Button("Sync Now"))
            {
                var undoTargets = new List<Object> { target };

                var resolvedTargets = follower.targetParent != null
                    ? GetChildren(follower.targetParent, follower.findTargetChild, follower.targetChildPath)
                    : follower.targets;

                if (resolvedTargets != null)
                {
                    foreach (var syncedTarget in resolvedTargets.Where(syncedTarget => syncedTarget != null))
                    {
                        undoTargets.Add(syncedTarget);
                        undoTargets.Add(syncedTarget.gameObject);
                        undoTargets.AddRange(syncedTarget.GetComponents<Collider>());
                    }
                }

                InariUdonEditorUtility.RecordAndDirty("ScaledMultiFollower Sync Now", undoTargets, () => SyncNow(follower));
                serializedObject.Update();
            }
        }

        private static bool ShouldDrawProperty(string propertyName, ScaledMultiFollower follower)
        {
            switch (propertyName)
            {
                case nameof(ScaledMultiFollower.sources):
                    return follower.sourceParent == null;
                case nameof(ScaledMultiFollower.targets):
                    return follower.targetParent == null;
                default:
                    return true;
            }
        }
    }
}
