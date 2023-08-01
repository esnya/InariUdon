using UdonSharp;
using UnityEngine;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace InariUdon.DynamicDownloaders
{
    /// <summary>
    /// Download images from specified VRCUrls and provides them as textures to many displays.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MultiImageManager : UdonSharpBehaviour
    {
        /// <summary>
        /// Urls of the image to be downloaded.
        /// </summary>
        public VRCUrl[] urls = { };

        /// <summary>
        /// Minimum delay (in seconds) before the image download begins.
        /// </summary>
        public float minDelay = 10;

        /// <summary>
        /// Maximum delay (in seconds) before the image download begins.
        /// </summary>
        public float maxDelay = 30;

        /// <summary>
        /// Minimum interval in seconds between to show next image.
        /// </summary>
        public float minInterval = 0;

        /// <summary>
        /// Maximum interval in seconds between to show next image.
        /// </summary>
        public float maxInterval = 5;

        /// <summary>
        /// Minimum delay (in seconds) before the next image download begins after error.
        /// </summary>
        public float minRetryDelay = 0;

        /// <summary>
        /// Maximum delay (in seconds) before the next image download begins after error.
        /// </summary>
        public float maxRetryDelay = 1;

        /// <summary>
        /// Information tells how convert downlded image as texture.
        /// </summary>
        public TextureInfo textureInfo;

        /// <summary>
        /// Temporarily used internally to getting download the texture, and must not be null. It is not used to render the final material, and can be set to any valid material.
        /// </summary>
        public Material loaderMaterial;


        /// <summary>
        /// Exclude erroed url from next download.
        /// </summary>
        public bool excludeErroedUrl = true;

        [HideInInspector] public Texture[] textures;
        [HideInInspector] public bool[] errored;
        [HideInInspector] public bool[] requests;
        public bool isCompleted;
        private VRCImageDownloader downloader;

        private void Start()
        {
            textures = new Texture[urls.Length];
            errored = new bool[urls.Length];
            requests = new bool[urls.Length];
            downloader = new VRCImageDownloader();
            isCompleted = false;
            SendCustomEventDelayedSeconds(nameof(_DownloadNext), Random.Range(minDelay, maxDelay));
        }

        private int GetNextIndex()
        {
            int index = -1;
            int remaining = 0;
            for (var i = 0; i < urls.Length; i++)
            {
                if (textures[i] != null || excludeErroedUrl && errored[i]) continue;
                remaining += 1;
                if (index < 0) index = i;
                if (requests[index]) return index;
            }

            if (remaining == 0) return -1;

            return index;
        }

        public void _DownloadNext()
        {
            int index = GetNextIndex();
            if (index < 0)
            {
                isCompleted = true;
                Debug.Log("All images are downloaded.", gameObject);
                return;
            }

            downloader.DownloadImage(urls[index], loaderMaterial, (IUdonEventReceiver)this, textureInfo);
        }

        private int GetIndexOfUrl(VRCUrl url)
        {
            for (var i = 0; i < urls.Length; i++)
            {
                if (urls[i] == url) return i;
            }

            Debug.LogError($"Url {url} is not found.", gameObject);

            return -1;
        }

        public override void OnImageLoadSuccess(IVRCImageDownload result)
        {
            var index = GetIndexOfUrl(result.Url);
            if (index < 0) return;

            textures[index] = result.Material.GetTexture(textureInfo.MaterialProperty);

            SendCustomEventDelayedSeconds(nameof(_DownloadNext), Random.Range(minInterval, maxInterval));
        }

        public override void OnImageLoadError(IVRCImageDownload result)
        {
            var index = GetIndexOfUrl(result.Url);
            if (index < 0) return;

            errored[index] = true;

            SendCustomEventDelayedSeconds(nameof(_DownloadNext), Random.Range(minRetryDelay, maxRetryDelay));
        }
    }
}
