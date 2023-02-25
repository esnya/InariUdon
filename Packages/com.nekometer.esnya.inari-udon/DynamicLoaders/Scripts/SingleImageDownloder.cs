
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace InariUdon.DynamicDownloaders
{
    /// <summary>
    /// Download an image from a specified VRCUrl and apply it as a texture to a specified material on a renderer.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SingleImageDownloder : UdonSharpBehaviour
    {
        /// <summary>
        /// Url of the image to be downloaded.
        /// </summary>
        public VRCUrl url;

        /// <summary>
        /// Index of the material on the renderer that the downloaded texture should be applied to.
        /// </summary>
        public int materialIndex;

        /// <summary>
        /// Minimum delay (in seconds) before the image download begins.
        /// </summary>
        public float minDelay = 0;

        /// <summary>
        /// Maximum delay (in seconds) before the image download begins.
        /// </summary>
        public float maxDelay = 30;

        /// <summary>
        /// Information tells how convert downlded image as texture.
        /// </summary>
        public TextureInfo textureInfo;

        /// <summary>
        /// Temporarily used internally to getting download the texture, and must not be null. It is not used to render the final material, and can be set to any valid material.
        /// </summary>
        public Material loaderMaterial;

        private void Start()
        {
            SendCustomEventDelayedSeconds(nameof(_Download), Random.Range(minDelay, maxDelay));
        }

        /// <summary>
        /// Invoke dowloading.
        /// </summary>
        public void _Download()
        {
            new VRCImageDownloader().DownloadImage(url, loaderMaterial, (IUdonEventReceiver)this, textureInfo);
        }

        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            var properties = new MaterialPropertyBlock();
            var renderer = GetComponent<MeshRenderer>();
            var propertyName = result.TextureInfo.MaterialProperty;

            renderer.GetPropertyBlock(properties, materialIndex);
            properties.SetTexture(propertyName, result.Material.GetTexture(propertyName));
            renderer.SetPropertyBlock(properties, materialIndex);

            RendererExtensions.UpdateGIMaterials(renderer);
        }

        public override void OnImageLoadError(IVRCImageDownload result)
        {
            Debug.LogError(result, this);
        }
    }
}
