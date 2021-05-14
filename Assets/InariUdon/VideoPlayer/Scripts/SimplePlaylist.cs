using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
using TMPro;
using UdonSharp;
using UdonToolkit;
using UnityEditor;
using UnityEngine;
using VRC.Core;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDKBase;
using VRC.Udon;

namespace EsnyaFactory.InariUdon
{
    [RequireComponent(typeof(VRCUnityVideoPlayer))]
    public class SimplePlaylist : UdonSharpBehaviour
    {
        #region Public Variables
        [ListView("Playlist")] public VRCUrl[] playlist = { };
        [Tooltip("Optional"), ListView("Playlist")] public string[] titles = { };
        public bool autoPlay = true;
        public float timeSkipThreshold = 0.5f;
        [Tooltip("Optional")] public TextMeshPro text;
        #endregion

        #region Unity Events
        VRCUnityVideoPlayer unityVideoPlayer;
        private void Start()
        {
            unityVideoPlayer = (VRCUnityVideoPlayer)GetComponent(typeof(VRCUnityVideoPlayer));
        }
        #endregion

        #region Udon Events
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player.isLocal && player.isMaster) LoadURL();
        }

        [UdonSynced] int syncIndex;
        [UdonSynced] float syncTime;
        [UdonSynced] bool syncPlaying;
        public override void OnPreSerialization()
        {
            syncPlaying = playing;
            syncIndex = index;
            syncTime = unityVideoPlayer.GetTime();
        }

        public override void OnDeserialization()
        {
            if (syncIndex != index) SetIndex(syncIndex);
            else if (syncPlaying && !playing) Play();
            else if (!syncPlaying && playing) Stop();
            else if (Mathf.Abs(syncTime - unityVideoPlayer.GetTime()) >= timeSkipThreshold) unityVideoPlayer.SetTime(syncTime);
        }
        #endregion

        #region Udon Video Events
        private string videoError, videoState;
        private bool playing;
        public override void OnVideoReady()
        {
            if (videoState != "Playing") videoState = "Ready";
            UpdateText();
            Play();
        }

        public override void OnVideoPause()
        {
            videoState = "Paused";
            UpdateText();
        }

        public override void OnVideoEnd()
        {
            videoState = "End";
            UpdateText();
            PlayNext();
        }

        public override void OnVideoStart()
        {
            videoState = "Playing";
            UpdateText();
        }

        public override void OnVideoPlay()
        {
            playing = true;
            videoState = "Playing";
            UpdateText();
        }

        public override void OnVideoError(VideoError error)
        {
            videoState = "Error";
            videoError = error.ToString();
            UpdateText();
        }
        #endregion

        #region Custom Events

        private int index = 0;
        private float lastLoadedTime = 0;
        private VRCUrl url;
        public void LoadURL()
        {
            if (url == playlist[index]) return;

            var time = Time.time;
            var pastTime = time - lastLoadedTime;
            if (pastTime <= 5.0f)
            {
                videoState = "Waiting";
                SendCustomEventDelayedSeconds(nameof(LoadURL), 6.0f - pastTime);
                UpdateText();
                return;
            }

            lastLoadedTime = time;
            url = playlist[index];
            unityVideoPlayer.LoadURL(url);

            videoState = "Loading";
            UpdateText();
        }

        public void Play()
        {
            playing = true;
            unityVideoPlayer.Play();
        }

        public void Stop()
        {
            playing = false;
            unityVideoPlayer.Stop();
        }

        public void SetIndex(int i)
        {
            var nextIndex = (i % playlist.Length + playlist.Length) % playlist.Length;
            if (nextIndex == index) return;
            index = nextIndex;
            LoadURL();
        }

        public void PlayNext()
        {
            SetIndex(index + 1);
        }

        public void PlayPrevious()
        {
            SetIndex(index - 1);
        }
        #endregion

        #region Internel Logics
        private void UpdateText()
        {
            if (text == null) return;

            var statusLine = videoState;
            if (videoState == "Error") statusLine = $"<color=red>Error: {videoError}</color>";

            var titleLine = titles != null && index < titles.Length ? titles[index] : "";

            text.text = $"{statusLine}\n{titleLine}";
        }
        #endregion
    }
}
