
using UdonSharp;
using VRC.SDKBase;

namespace EsnyaFactory.InariUdon.Player
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class PlayerEventLogger : UdonSharpBehaviour
    {
        #region Public Variables
        public UI.UdonLogger logger;
        public string level = "NOTICE";
        public string joinedFormat = "{0} <color=green>joined</color> (Total {1})";
        public string leftFormat = "{0} <color=orange>left</color> (Total {1})";
        #endregion

        #region Unity Events
        void Start()
        {
            Log("Info", "Initialized");
        }
        #endregion

        #region Udon Events
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            Log("Info", string.Format(joinedFormat, player.displayName, VRCPlayerApi.GetPlayerCount()));
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            Log("Info", string.Format(leftFormat, player.displayName, VRCPlayerApi.GetPlayerCount() - 1));
        }
        #endregion

        #region UdonLogger
        private void Log(string level, string log)
        {
            logger.Log(level, gameObject.name, log);
        }
        #endregion
    }
}
