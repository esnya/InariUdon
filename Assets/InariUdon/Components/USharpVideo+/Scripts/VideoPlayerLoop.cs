using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.AVPro;

namespace EsnyaFactory.InariUdon
{
    public class VideoPlayerLoop : UdonSharpBehaviour
    {
        public VRCUnityVideoPlayer unityVideoPlayer;
        public VRCAVProVideoPlayer avProVideoPlayer;
        public Image icon;
        public bool loop = false;

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player.isLocal) UpdateVideoPlayer();
        }

        public void Toggle() {
            loop = !loop;
            UpdateVideoPlayer();
        }

        void UpdateVideoPlayer()
        {
            if (unityVideoPlayer != null) unityVideoPlayer.Loop = loop;
            if (avProVideoPlayer != null) avProVideoPlayer.Loop = loop;
            if (icon != null) icon.color = loop ? Color.white : Color.gray;
        }
    }
}
