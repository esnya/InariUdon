using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace InariUdon.UdonSharpVideoPlus
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class UdonToggle : UdonSharpBehaviour
    {
        public UdonSharpBehaviour target;
        public Image buttonImage;

        private Color buttonColor;

        private void Start()
        {
            if (buttonImage != null)
            {
                buttonColor = buttonImage.color;
            }
            UpdateButtonColor();
        }

        public void _Toggle()
        {
            target.enabled = !target.enabled;
            UpdateButtonColor();
        }

        private void UpdateButtonColor()
        {
            if (buttonImage == null) return;
            buttonImage.color = target.enabled ? buttonColor : buttonColor * 0.5f;
        }
    }
}
