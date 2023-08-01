
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Image;
using VRC.SDKBase;

namespace InariUdon.DynamicDownloaders
{
    /// <summary>
    /// Download an image from a specified VRCUrl and apply it as a texture to a specified material on a renderer.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MultiImageDownloder : AbstractSingleImageDownloder
    {
        /// <summary>
        /// Urls of the image to be downloaded.
        /// </summary>
        public VRCUrl[] urls = { };

        /// <summary>
        /// Shuffle order of urls.
        /// </summary>
        public bool shuffle;

        /// <summary>
        /// Minimum interval in seconds between to show next image.
        /// </summary>
        public float minInterval = 30;

        /// <summary>
        /// Maximum interval in seconds between to show next image.
        /// </summary>
        public float maxInterval = 30;

        /// <summary>
        /// Cache downloaded image.
        /// </summary>
        public bool cache = true;

        /// <summary>
        /// Minimum delay (in seconds) before the next image download begins after error.
        /// </summary>
        public float minRetryDelay = 0;

        /// <summary>
        /// Maximum delay (in seconds) before the next image download begins after error.
        /// </summary>
        public float maxRetryDelay = 1;

        /// <summary>
        /// Exclude erroed url from next download.
        /// </summary>
        public bool excludeErroedUrl = true;

        private int Index => shuffle ? indices[index] : index;
        public override VRCUrl ImageUrl => urls[index];
        private int[] indices;
        private int index;
        private Texture[] textures;
        private float nextUpdateTime;
        private bool[] erroed;

        private void Start()
        {
            indices = new int[urls.Length];
            textures = new Texture[urls.Length];
            erroed = new bool[urls.Length];
            for (var i = 0; i < urls.Length; i++) indices[i] = i;
            if (shuffle) Shuffle();
            _DownloadDelayed();
        }

        private void Shuffle()
        {
            Utilities.ShuffleArray(indices);
        }

        public override void _Download()
        {
            Schedule();

            var cachedTexture = textures[Index];

            if (cachedTexture) ApplyTexture(cachedTexture);
            else base._Download();

            IncrementIndex();
        }

        private void IncrementIndex()
        {
            for (var i = 0; i < urls.Length; i++)
            {
                if (++index >= urls.Length)
                {
                    index = 0;
                    if (shuffle) Shuffle();
                }
                if (!excludeErroedUrl || !erroed[Index]) break;
            }
        }

        private void Schedule()
        {
            nextUpdateTime = Time.time + Random.Range(minInterval, maxInterval);
        }

        private void OnRenderObject()
        {
            if (Time.time >= nextUpdateTime) _Download();
        }

        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            base.OnImageLoadSuccess(result);
            textures[Index] = result.Material.GetTexture(textureInfo.MaterialProperty);
        }

        public override void OnImageLoadError(IVRCImageDownload result)
        {
            base.OnImageLoadError(result);

            if (excludeErroedUrl) erroed[Index] = true;

            IncrementIndex();
            SendCustomEventDelayedSeconds(nameof(_Download), Random.Range(minRetryDelay, maxRetryDelay));
        }
    }
}
