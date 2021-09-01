using UdonSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace InariUdon.UdonSharpVideoPlus
{
    [
        DefaultExecutionOrder(100),
        UdonBehaviourSyncMode(BehaviourSyncMode.Manual),
    ]
    public class AudioModeToggle : UdonSharpBehaviour
    {
        public UdonSharpBehaviour usharpVideoPlayer;
        public AudioSource[] audioSources = {};
        public RectTransform sliderTransform;
        public GameObject lockedOverlay;
        public GameObject lockIcon;

        private bool _locked;
        private bool Locked {
            set
            {
                if (_locked == value) return;
                _locked = value;
                if (lockedOverlay != null) lockedOverlay.SetActive(value);
            }
            get => _locked;
        }
        public bool default2DMode;
        private Animator animator;
        private Text sliderText;
        private AnimationCurve[] spatialCurves;
        private bool allowInstanceCreatorControl = true;

        [UdonSynced, FieldChangeCallback(nameof(Spatialize))] private bool _spatialize;
        private bool Spatialize {
            set
            {
                _spatialize = value;
                animator.SetInteger("Target", value ? 0 : 1);
                sliderText.text = value ? "3D" : "2D";

                if (audioSources != null)
                {
                    for (var i = 0; i < audioSources.Length; i++)
                    {
                        var audioSource = audioSources[i];
                        if (audioSource == null) continue;

                        if (value) audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, spatialCurves[i]);
                        else audioSource.spatialBlend = 0;
                    }
                }
            }
            get => _spatialize;
        }


        private void Start()
        {
            animator = GetComponent<Animator>();
            sliderText = sliderTransform.GetComponentInChildren<Text>();

            spatialCurves = new AnimationCurve[audioSources.Length];
            for (int i = 0; i < audioSources.Length; i++)
            {
                var audioSource = audioSources[i];
                if (audioSource == null) continue;
                spatialCurves[i] = audioSource.GetCustomCurve(AudioSourceCurveType.SpatialBlend);
            }

            if (usharpVideoPlayer != null) allowInstanceCreatorControl = (bool)usharpVideoPlayer.GetProgramVariable(nameof(allowInstanceCreatorControl));

            Spatialize = !default2DMode;
        }

        private void Update()
        {
            if (lockIcon != null)
            {
                var player = Networking.LocalPlayer;
                Locked = !player.isMaster && (!allowInstanceCreatorControl || !player.isInstanceOwner) && lockIcon.activeSelf;
            }
        }

        public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
        {
            return false;
        }

        public void Toggle()
        {
            if (Networking.IsOwner(gameObject))
            {
                Spatialize = !Spatialize;
                RequestSerialization();
            }
            else if (!Locked) SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(Toggle));
        }
    }
}
