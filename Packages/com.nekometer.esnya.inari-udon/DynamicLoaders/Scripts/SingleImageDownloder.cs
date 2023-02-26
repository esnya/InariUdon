
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
    public class SingleImageDownloder : AbstractSingleImageDownloder
    {
        /// <summary>
        /// Url of the image to be downloaded.
        /// </summary>
        public VRCUrl url;

        public override VRCUrl ImageUrl => url;

        private void Start()
        {
            _DownloadDelayed();
        }
    }
}
