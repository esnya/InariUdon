namespace EsnyaFactory
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    public class PhysicalSwitchTrigger : UdonSharpBehaviour
    {
        public bool state;
        public UdonBehaviour target;
        public string variableName = "state";
        public string eventName = "OnValueChanged";

        private void OnTriggerEnter(Collider other)
        {
            if (target == null) return;
            target.SetProgramVariable("state", state);
            target.SendCustomEvent(eventName);
        }
    }
}
