
using UdonSharp;
using UdonToolkit;
using UnityEngine;

namespace EsnyaFactory.InariUdon
{
    [CustomName("Set Parent")]
    public class SetParent : UdonSharpBehaviour
    {
        #region Public Variables
        [HideIf("@findParentByName")] public Transform parent;
        [HideIf("@!findParentByName")] public string parentName;
        public bool findParentByName;
        [Tooltip("Use this to set null.")] public Transform target;
        public bool keepGrobalTransform;
        public bool triggerOnStart;
        #endregion

        #region Internal Variables
        bool initialized;
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
