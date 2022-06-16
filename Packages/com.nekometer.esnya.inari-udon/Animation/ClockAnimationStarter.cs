
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace InariUdon
{
    /// <summary>
    /// Adjust animation time to current time.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ClockAnimationStarter : UdonSharpBehaviour
    {
        /// <summary>
        /// Target animator
        ///
        /// Get from attached object or parents if null
        /// </summary>
        public Animator clockAnimator;

        /// <summary>
        /// Use utc if true
        /// </summary>
        public bool useUTC = false;

        /// <summary>
        /// Layer name in animator controller
        /// </summary>
        public string layerName = "Default";

        /// <summary>
        /// State name in animator controller
        /// </summary>
        public string stateName = "Clock";

        private void Start()
        {
            if (!clockAnimator) clockAnimator = GetComponentInParent<Animator>();

            var layerId = clockAnimator.GetLayerIndex(layerName);;
            var time = useUTC ? DateTime.Now : DateTime.UtcNow;
            var normalizedTime = Convert.ToSingle(time.TimeOfDay.TotalHours / 12.0 % 1.0);

            clockAnimator.Play(stateName, layerId, normalizedTime);

            enabled = false;
        }
    }
}
