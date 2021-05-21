using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace EsnyaFactory.InariUdon
{
  [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
  public class VideoScreenBrightness : UdonSharpBehaviour
  {
    public Slider slider;
    public MeshRenderer screen;
    public int subMesh = 0;
    public string colorPropertyName = "_EmissionColor";
    public Image icon;

    private Material material;
    private Color max;

    private void Start() {
      material = screen.materials[subMesh];
      max = material.GetColor(colorPropertyName);
    }

    public void SliderValueChanged() {
      var value = slider.value;
      material.SetColor(colorPropertyName, max * value);
      if (icon != null) icon.color = Color.white * value;
    }
  }
}
