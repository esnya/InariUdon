

using UdonSharp;
using UdonToolkit;
using UnityEngine.UI;
using VRC.SDKBase;

namespace EsnyaFactory {
    [CustomName("Player Count Display")]
    [HelpMessage("Display player count using UI.Text. Displays world max capacity if provided.")]
    public class PlayerCountDisplay : UdonSharpBehaviour
    {
        public Text targetText;
        public int maxCapacity = 0;

        private VRCPlayerApi[] players;
        void Start()
        {
            players = new VRCPlayerApi[maxCapacity > 0 ? maxCapacity * 2 : 80];
            UpdateText();
        }

        void UpdateText()
        {
            var count = VRCPlayerApi.GetPlayers(players).Length;
            targetText.text = maxCapacity > 0 ? $"{count}/{maxCapacity} ({maxCapacity * 2})" : $"{count}";
        }

        public override void OnPlayerJoined(VRCPlayerApi player) {
            UpdateText();
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            UpdateText();
        }
    }
}