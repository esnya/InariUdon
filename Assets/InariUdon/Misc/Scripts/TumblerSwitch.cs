namespace EsnyaFactory {
  using UdonSharp;
  using UnityEngine;
  using VRC.SDKBase;
  using VRC.Udon;
  using VRC.Udon.Common.Interfaces;

  [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]

  public class TumblerSwitch : UdonSharpBehaviour
  {
    public Animator localAnimator;
    public string localParameter = "State";

    public AudioSource audioSource;

    public Animator externalAnimator;
    public string externalParameter = "State";

    public bool state;
    public bool sync;

    public UdonBehaviour customEventTarget;
    public string customTurnOnEventName = "TurnOn";
    public string customTurnOffEventName = "TurnOff";

    private void Start()
    {
      if (localAnimator == null)
      {
        localAnimator = GetComponent<Animator>();
      }

      if (audioSource == null)
      {
        audioSource = GetComponent<AudioSource>();
      }

      if (sync) {
        if (Networking.IsOwner(gameObject)) {
          SetState(state);
        }
      }
    }

    public override void OnPlayerJoined(VRCPlayerApi player) {
      if (sync && Networking.IsOwner(gameObject)) {
        Sync();
      }
    }

    override public void Interact()
    {
      if (sync) SyncedToggle();
      else LocalToggle();

      SendCustomNetworkEvent(NetworkEventTarget.All, "PlayAudio");
    }

    private void LocalToggle()
    {
      if (state) TurnOff();
      else TurnOn();
    }

    private void SyncedToggle()
    {
      if (state) SendCustomNetworkEvent(NetworkEventTarget.All, "TurnOff");
      else SendCustomNetworkEvent(NetworkEventTarget.All, "TurnOn");
    }

    public void Sync()
    {
      if (state) SendCustomNetworkEvent(NetworkEventTarget.All, "TurnOn");
      else SendCustomNetworkEvent(NetworkEventTarget.All, "TurnOff");
    }

    private void SetState(bool newState) {
      state = newState;
      if (localAnimator != null) localAnimator.SetBool(localParameter, state);
      if (externalAnimator != null) externalAnimator.SetBool(externalParameter, state);
    }

    public void PlayAudio()
    {
      if (audioSource != null) audioSource.Play();
    }

    public void TurnOn()
    {
      SetState(true);
      if (customEventTarget) customEventTarget.SendCustomEvent(customTurnOnEventName);
    }

    public void TurnOff()
    {
      SetState(false);
      if (customEventTarget) customEventTarget.SendCustomEvent(customTurnOffEventName);
    }
  }
}
