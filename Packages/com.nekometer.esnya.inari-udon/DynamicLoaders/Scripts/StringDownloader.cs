
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace InariUdon.DynamicDownloaders
{
    /// <summary>
    /// downloads a string from a specified URL and writes it to a Unity Text component.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class StringDownloader : UdonSharpBehaviour
    {
        /// <summary>
        /// URL to download the string from.
        /// </summary>
        public VRCUrl url;

        /// <summary>
        /// Minimum delay in seconds before downloading.
        /// </summary>
        public float minDelay = 10;

        /// <summary>
        /// Maximum delay in seconds before downloading.
        /// </summary>
        public float maxDelay = 30;

        [Header("Write Target")]
        /// <summary>
        /// Component to write the downloaded string to.
        /// </summary>
        public Text uiText;

        /// <summary>
        /// Component to write the downloaded string to.
        /// </summary>
        public TextMeshPro textMeshPro;

        /// <summary>
        /// Component to write the downloaded string to.
        /// </summary>
        public TextMeshProUGUI textMeshProUGUI;

        private void Start()
        {
            SendCustomEventDelayedSeconds(nameof(_Download), Random.Range(minDelay, maxDelay));
        }

        /// <summary>
        /// Invokes downloading.
        /// </summary>
        public void _Download()
        {
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            var text = result.Result;

            if (uiText) uiText.text = text;
            if (textMeshPro) textMeshPro.text = text;
            if (textMeshProUGUI) textMeshProUGUI.text = text;
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            Debug.LogError(result, this);
        }
    }
}
