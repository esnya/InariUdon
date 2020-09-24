namespace EsnyaFactory {
  using UdonSharp;
  using UnityEngine;
  using VRC.SDKBase;
  using VRC.Udon;

  public class EntranceSound : UdonSharpBehaviour
  {
      public AudioSource joined;
      public AudioSource left;

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
      joined.Play();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
      left.Play();
    }
  }
}
