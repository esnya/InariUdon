
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace EsnyaFactory.InariUdon
{
    [DefaultExecutionOrder(1000), UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class DefaultStreamURL : UdonSharpBehaviour
    {
        public UdonSharpBehaviour usharpVideo;
        public VRCUrl url;

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player.isMaster && player.isLocal)
            {
                var inputField = (VRCUrlInputField)usharpVideo.GetProgramVariable("inputField");
                Debug.Log("Set Stream Sync Mode");
                usharpVideo.SendCustomEvent("SetStreamSyncMode");
                Debug.Log("Set Url");
                inputField.SetUrl(url);
                Debug.Log("Handle URL Input");
                usharpVideo.SendCustomEvent("HandleURLInput");
            }
        }
    }
}
