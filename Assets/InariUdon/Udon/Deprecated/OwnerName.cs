
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace EsnyaFactory {
    public class OwnerName : UdonSharpBehaviour
    {
        public GameObject target;
        public UnityEngine.UI.Text text;

        private void Update() {
            if (text != null) {
                var owner = Networking.GetOwner(target);
                if (owner != null) {
                    text.text = owner.displayName;
                }
            }
        }
    }
}
