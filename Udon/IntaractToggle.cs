
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace EsnyaFactory {
    public class IntaractToggle : UdonSharpBehaviour
    {
        public GameObject toggle;
        public UdonBehaviour customEventTarget;
        public string customEventName;

        public override void Interact() {
            if (toggle != null) {
                toggle.SetActive(!toggle.activeSelf);
            }

            if (customEventTarget != null) {
                customEventTarget.SendCustomEvent(customEventName);
            }
        }
    }
}
