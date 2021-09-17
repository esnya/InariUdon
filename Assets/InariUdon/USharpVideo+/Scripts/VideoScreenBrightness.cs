using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using UdonToolkit;

namespace InariUdon.UdonSharpVideoPlus
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class VideoScreenBrightness : UdonSharpBehaviour
    {
        public Slider slider;
        public MeshRenderer screen;
        public int subMesh = 0;
        public string colorPropertyName = "_EmissionColor";
        public bool uiBrightness;
        [HideIf("@uiBrightness")] public Image icon;
        [HideIf("@!uiBrightness")] public Material uiMaterial;

        private Material material;
        private Color max, uiMax;

        private void Start() {
            material = screen.materials[subMesh];
            max = material.GetColor(colorPropertyName);
            uiMax = uiMaterial != null ? uiMaterial.GetColor("_Color") : Color.white;
        }

        public void OnSliderValueChanged() {
            var value = slider.value;
            var color = Color.white * value;
            color.a = 1.0f;

            material.SetColor(colorPropertyName, max * color);

            if (uiBrightness)
            {
                if (uiMaterial != null)
                {
                    color.a = 1.0f;
                    uiMaterial.SetColor("_Color", uiMax * color);
                }
            }
            else if (icon != null) icon.color = Color.white * color;
        }
    }
}
