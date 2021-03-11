using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;

namespace EsnyaFactory
{
    [CustomName("Entrance Sound Player")]
    [HelpMessage("Play sound using AudioSource when player joined or left. To disable either of them, select None.")]
    public class EntranceSoundPlayer : UdonSharpBehaviour
    {
        public AudioSource joinedSoundSource;
        public AudioSource leftSoundSource;

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (joinedSoundSource != null) joinedSoundSource.Play();
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (leftSoundSource != null) leftSoundSource.Play();
        }
    }
}
