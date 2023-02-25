using System;
using UdonSharp;
using UnityEngine;


namespace InariUdon.Driver
{
    /// <summary>
    /// Controls the rotation of the clock's seconds, minutes, and hours hands using the current time.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ClockDriver : UdonSharpBehaviour
    {
        /// <summary>
        /// The transform of the seconds hand of the clock.
        /// </summary>
        public Transform seconds;
        /// <summary>
        /// The transform of the minutes hand of the clock.
        /// </summary>
        public Transform minutes;
        /// <summary>
        /// The transform of the hours hand of the clock.
        /// </summary>
        public Transform hours;
        /// <summary>
        /// The axis around which the hands of the clock should rotate.
        /// </summary>
        public Vector3 axis = Vector3.forward;

        /// <summary>
        /// Indicates whether the clock should use the local time (true) or the UTC (false).
        /// </summary>
        public bool localTime = true;
        private int prevTime;

        private const int DegPerSec = 360 / 60;
        private const float DegPerMin = DegPerSec / 60.0f;
        private const float DegPerHr = DegPerMin / 12.0f;

        private void Update()
        {
            var time = Mathf.FloorToInt((float)(localTime ? DateTime.Now : DateTime.UtcNow).TimeOfDay.TotalSeconds);
            if (time == prevTime) return;

            seconds.localRotation = Quaternion.AngleAxis(time * DegPerSec, axis);
            minutes.localRotation = Quaternion.AngleAxis(time * DegPerMin, axis);
            hours.localRotation = Quaternion.AngleAxis(time * DegPerHr, axis);

            prevTime = time;
        }
    }
}
