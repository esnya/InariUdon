#pragma warning disable IDE1006

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace InariUdon.Trigger
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class InputTrigger : UdonSharpBehaviour
    {
        public string keyName = "return";
        public string buttonName = "Oculus_CrossPlatform_Button4";
        [Header("OnDown Events")]
        public bool keyDown = true;
        public bool buttonDown = true;
         public UdonSharpBehaviour[] eventTargets;
        public string[] eventNames;

        [Header("OnUp Events")]
        public bool keyUp = false;
        public bool buttonUp = false;
         public UdonSharpBehaviour[] onUpEventTargets;
        public string[] onUpEventNames;

        private void Update()
        {
            if (keyDown && Input.GetKeyDown(keyName) || buttonDown && Input.GetButtonDown(buttonName)) _SendEvents(eventTargets, eventNames);
            if (keyUp && Input.GetKeyUp(keyName) || buttonUp && Input.GetButtonUp(buttonName)) _SendEvents(onUpEventTargets, onUpEventNames);
        }

        private void _SendEvents(UdonSharpBehaviour[] eventTargets, string[] eventNames)
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
