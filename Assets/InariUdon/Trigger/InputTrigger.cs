#pragma warning disable IDE1006

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonToolkit;

namespace InariUdon.Trigger
{
    [HelpMessage("Trigger by input")]
    public class InputTrigger : UdonSharpBehaviour
    {
        public bool keyDown = true;
        public string keyName = "return";

        public bool buttonDown = true;
        public string buttonName = "Oculus_CrossPlatform_Button4";

        [ListView("Event Targets")] public UdonSharpBehaviour[] eventTargets;
        [ListView("Event Targets"), Popup("behaviour", "@eventTargets", true)] public string[] eventNames;

        private void Update()
        {
            if (keyDown && Input.GetKeyDown(keyName) || buttonDown && Input.GetButtonDown(buttonName)) _Trigger();
        }

        public void _Trigger()
        {
            if (eventTargets != null && eventNames != null)
            {
                var count = Mathf.Min(eventTargets.Length, eventNames.Length);
                for (int i = 0; i < count; i++)
                {
                    var target = eventTargets[i];
                    if (target == null) continue;
                    target.SendCustomEvent(eventNames[i]);
                }
            }
        }

        public void _Enable() => enabled = true;
        public void _Disable() => enabled = false;
    }
}
