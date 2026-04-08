using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace InariUdon.Player
{
    
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
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
