
using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;

namespace EsnyaFactory.InariUdon
{
    public class PlayerEventLogger : UdonSharpBehaviour
    {
        #region Internal Variables
        bool initialized;
        #endregion

        #region Unity Events
        void Start()
        {
            initialized = true;
            Log("Info", "Initialized");
        }
        #endregion

        #region Udon Events
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            Log("Info", $"{player.displayName} Joined");
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            Log("Info", $"{player.displayName} Left");
        }
        #endregion

        #region UdonLogger
        [Space][SectionHeader("Udon Logger")][UTEditor]
        public UdonLogger logger;
        bool loggerInitialized;
        void Log(string level, string log)
        {
            if (!loggerInitialized)
            {
                if (logger.initialized)
                {
                    loggerInitialized = true;
                }
                else
                {
                    return;
                }
            }

            logger.Log(level, gameObject.name, log);
        }
        #endregion
    }
}
