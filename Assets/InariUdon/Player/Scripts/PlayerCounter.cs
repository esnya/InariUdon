

using UdonSharp;
using UdonToolkit;
using UnityEngine.UI;
using VRC.SDKBase;
using TMPro;

namespace EsnyaFactory.InariUdon {
    [CustomName("Player Counter")]
    [HelpMessage("Display player count using UI.Text. Displays world max capacity if provided.")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class PlayerCounter : UdonSharpBehaviour
    {
        public TextMeshPro text;
        public int maxCapacity = 0;

        private VRCPlayerApi[] players;

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
