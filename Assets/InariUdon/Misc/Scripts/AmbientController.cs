namespace EsnyaFactory {
  using UdonSharp;
  using UnityEngine;
  using UnityEngine.Rendering;
  using VRC.SDKBase;
  using VRC.Udon;

  public class AmbientController : UdonSharpBehaviour
  {
    public AmbientMode ambientMode = AmbientMode.Skybox;
    public Color ambientEquatorColor = Color.gray;
    public Color ambientGroundColor = Color.gray;
    public float ambientIntensity = 1;
    public Color ambientLight = Color.gray;
    public Color ambientSkyColor = Color.gray;

    public void Apply()
    {
      RenderSettings.ambientMode = ambientMode;
      RenderSettings.ambientEquatorColor = ambientEquatorColor;
      RenderSettings.ambientGroundColor = ambientGroundColor;
      RenderSettings.ambientIntensity = ambientIntensity;
      RenderSettings.ambientLight = ambientLight;
      RenderSettings.ambientSkyColor = ambientSkyColor;
    }
  }
}
