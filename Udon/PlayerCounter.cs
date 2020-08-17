

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerCounter : UdonSharpBehaviour
{
    public Text target;
    private int count = 0;
    public int maxCapacity = -1;

    void Update()
    {
        target.text = maxCapacity >= 0 ? $"{count}/{maxCapacity}" : $"{count}";
    }

    public override void OnPlayerJoined(VRCPlayerApi player) {
        count++;
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        count--;
    }
}
