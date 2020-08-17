
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SyncedToggle : UdonSharpBehaviour
{
    public GameObject toggle;
    [UdonSynced] public bool state = true;

    private bool isDirty = false;
    private bool newState = true;

    void Start()
    {
        newState = state;
        isDirty = false;
    }

    void Update() {
        if (isDirty) {
            if (Networking.IsOwner(gameObject)) {
                state = newState;
                isDirty = false;
            } else {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
        }

        if (toggle != null) {
            toggle.SetActive(state);
        }
    }

    public void Toggle() {
        newState = !state;
        isDirty = true;
    }
}
