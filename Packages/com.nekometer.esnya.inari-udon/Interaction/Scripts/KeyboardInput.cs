using UdonSharp;
using UnityEngine;

namespace InariUdon.Interaction
{
    /// <summary>
    /// Listens for keyboard input and triggers specified events on specified targets.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class KeyboardInput : UdonSharpBehaviour
    {
        /// <summary>
        /// The key codes that will trigger the events.
        /// </summary>
        public int[] keyCodes = { };

        /// <summary>
        /// The modes of key input that will trigger the events. Use MODE_KEY_DOWN for key down events, MODE_KEY_UP for key up events, and MODE_KEY_HOLD for hold events.
        /// </summary>
        public KeyboardInputEventType[] modes = { };

        /// <summary>
        /// The targets of the events.
        /// </summary>
        public UdonSharpBehaviour[] eventTargets = { };

        /// <summary>
        /// The names of the events that will be triggered on the targets when the keys are pressed.
        /// </summary>
        public string[] onKeyDownEvents = { };

        /// <summary>
        /// The amount of time in seconds that a key must be held down to trigger a hold event.
        /// </summary>
        public float holdTime = 1.0f;

        /// <summary>
        /// The interval in seconds between each hold events.
        /// </summary>
        public float holdInterval = 0.5f;

        /// <summary>
        /// The audio source that will play a sound when an event is triggered.
        /// </summary>
        public AudioSource audioSource;

        private float[] holdTimers;
        private void Start()
        {
            holdTimers = new float[keyCodes.Length];
        }

        private void Update()
        {
            for (int i = 0; i < keyCodes.Length; i++)
            {
                var eventTarget = eventTargets[i];
                if (!eventTarget) continue;

                var keyCode = (KeyCode)keyCodes[i];
                var onKeyDownEvent = onKeyDownEvents[i];

                switch (modes[i])
                {
                    case KeyboardInputEventType.KeyDown:
                        if (Input.GetKeyDown(keyCode)) Trigger(eventTarget, onKeyDownEvent);
                        break;
                    case KeyboardInputEventType.KeyUp:
                        if (Input.GetKeyUp(keyCode)) Trigger(eventTarget, onKeyDownEvent);
                        break;
                    case KeyboardInputEventType.KeyHold:
                        if (Input.GetKeyDown(keyCode)) holdTimers[i] = Time.time + holdTime;
                        else if (Input.GetKey(keyCode) && Time.time >= holdTimers[i])
                        {
                            Trigger(eventTarget, onKeyDownEvent);
                            holdTimers[i] = Time.time + holdInterval;
                        }
                        break;
                }
            }
        }

        private void Trigger(UdonSharpBehaviour target, string eventName)
        {
            if (!target) return;
            target.SendCustomEvent(eventName);
            PlaySound();
        }

        private void PlaySound()
        {
            if (audioSource && audioSource.clip)
            {
                var obj = Instantiate(audioSource.gameObject);
                obj.transform.SetParent(transform, false);
                var spawnedAudioSource = obj.GetComponent<AudioSource>();
                spawnedAudioSource.Play();
                Destroy(obj, spawnedAudioSource.clip.length + 1.0f);
            }
        }
    }

    /// <summary>
    /// Enum representing the event types for keyboard input.
    /// </summary>
    public enum KeyboardInputEventType
    {
        /// <summary>
        /// The key has been pressed down.
        /// </summary>
        KeyDown,

        /// <summary>
        /// The key has been released.
        /// </summary>
        KeyUp,

        /// <summary>
        /// The key is being held down.
        /// </summary>
        KeyHold,
    }
}
