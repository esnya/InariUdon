

using UdonSharp;
using UdonToolkit;
using VRC.SDKBase;
using TMPro;
using UnityEngine;

namespace EsnyaFactory.InariUdon.Player {
    [
        CustomName("Player Counter"),
        HelpMessage("Display number of players in the instance with TextMeshPro. Alos show world max capacity if provided."),
        UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync),
    ]
    public class PlayerCounter : UdonSharpBehaviour
    {
        [Tooltip("TextMeshPro component to display counts.")] public TextMeshPro text;
        [Tooltip("Max capacity of world. Set 0 to disable.")] public int maxCapacity = 0;

        void UpdateText(int add)
        {
            var count = VRCPlayerApi.GetPlayerCount() + add;
            var str = maxCapacity > 0 ? $"{count}/<size=33%>{maxCapacity}</size>" : $"{count}";
            if (text != null) text.text = str;
        }

        public override void OnPlayerJoined(VRCPlayerApi player) {
            UpdateText(0);
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            UpdateText(-1);
        }
    }
}
