

using UdonSharp;
using UdonToolkit;
using UnityEngine.UI;
using VRC.SDKBase;

namespace EsnyaFactory {
    [CustomName("Player Counter")]
    [HelpMessage("Display player count using UI.Text. Displays world max capacity if provided.")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class PlayerCounter : UdonSharpBehaviour
    {
        public Text uiText;
        public TMPro.TextMeshPro tmproText;
        public int maxCapacity = 0;

        private VRCPlayerApi[] players;

        void UpdateText(int add)
        {
            var count = VRCPlayerApi.GetPlayerCount() + add;
            var text = maxCapacity > 0 ? $"{count}/<size=33%>{maxCapacity}</size>" : $"{count}";
            if (uiText != null) uiText.text = text;
            if (tmproText != null) tmproText.text = text;
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
