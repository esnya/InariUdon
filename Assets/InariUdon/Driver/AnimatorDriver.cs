
using UdonSharp;
using UnityEngine;

namespace InariUdon.Driver
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AnimatorDriver : UdonSharpBehaviour
    {
        public Animator animator;
        public string parameterName;
        public float floatValue;

        public void SetFloat()
        {
            if (animator == null) return;

            animator.SetFloat(parameterName, floatValue);
        }
    }
}