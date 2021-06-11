using UdonSharp;
using UdonToolkit;
using UnityEngine;

namespace EsnyaFactory.InariUdon.Transforms
{
    [
        CustomName("Set Parent"),
        HelpMessage("Modify parent in hierarchy ay runtime"),
        UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync),
    ]
    public class SetParent : UdonSharpBehaviour
    {
        #region Public Variables
        [HideIf("@findParentByName")] [UTEditor] public Transform parent;
        [HideIf("@!findParentByName")] [UTEditor] public string parentName;
        [Tooltip("Find parent by `GameObject.Find(parentName)`")] public bool findParentByName;
        [Tooltip("None to use `this.transform`")] public Transform target;

        public bool keepGrobalTransform;
        public bool triggerOnStart;
        #endregion

        #region Unity Events
        void Start()
        {
            if (findParentByName) parent = GameObject.Find(parentName).transform;
            if (target == null) target = transform;
            if (triggerOnStart) Trigger();
        }
        #endregion

        #region Udon Events
        #endregion

        #region Custom Events
        [Documentation.EventDescription("Set parent")]
        public void Trigger()
        {
            target.SetParent(parent);
            if (!keepGrobalTransform)
            {
                target.localPosition = Vector3.zero;
                target.localRotation = Quaternion.identity;
                target.localScale = Vector3.one;
            }
        }
        #endregion

        #region Internal Logics
        #endregion
    }
}
