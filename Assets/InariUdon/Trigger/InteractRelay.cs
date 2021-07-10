#pragma warning disable IDE1006

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonToolkit;

namespace InariUdon.Trigger
{
    [HelpMessage("Overrides Interact, or integrates multiple.")]
    public class InteractRelay : UdonSharpBehaviour
    {
        public UdonSharpBehaviour[] relayTargets;
        [ListView("Event Targets")] public UdonSharpBehaviour[] eventTargets;
        [ListView("Event Targets"), Popup("behaviour", "@eventTargets", true)] public string[] eventNames;

        public override void Interact() => _Trigger();

        public void _Trigger()
        {
            if (relayTargets != null)
            {
                foreach (var target in relayTargets) target.SendCustomEvent("_interact");
            }

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
