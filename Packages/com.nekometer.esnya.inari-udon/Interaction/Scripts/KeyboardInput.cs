using UdonSharp;
using UnityEngine;

namespace InariUdon.Interaction
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class KeyboardInput : UdonSharpBehaviour
    {
        private const int MODE_KEY_DOWN = 0;
        private const int MODE_KEY_UP = 1;
        private const int MODE_KEY_HOLD = 2;

        public int[] keyCodes = { };
        public int[] modes = { };
        public UdonSharpBehaviour[] eventTargets = { };
        public string[] onKeyDownEvents = { };
        public float holdTime = 1.0f;
        public float holdInterval = 0.5f;

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
                    case MODE_KEY_DOWN:
                        if (Input.GetKeyDown(keyCode)) Trigger(eventTarget, onKeyDownEvent);
                        break;
                    case MODE_KEY_UP:
                        if (Input.GetKeyUp(keyCode)) Trigger(eventTarget, onKeyDownEvent);
                        break;
                    case MODE_KEY_HOLD:
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
}
