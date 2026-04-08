using UdonSharp;
using UnityEngine;

namespace InariUdon.Rendering
{
    
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ReflectionProbeController : UdonSharpBehaviour
    {
        public GameObject reflectionProbe;
        public bool renderOnStart;
        ReflectionProbe _reflectionProbe; // ToDo

        void Start()
        {
            if (reflectionProbe != null) _reflectionProbe = GetComponent<ReflectionProbe>();

            if (_reflectionProbe == null) {
                Debug.LogError($"[{nameof(ReflectionProbe)}({gameObject.name})] reflectionProbe is requred");
            }

            if (renderOnStart) RenderProbe();
        }

        public void RenderProbe()
        {
            if (_reflectionProbe == null) return;

            _reflectionProbe.RenderProbe();

            Debug.Log($"[{nameof(ReflectionProbe)}({gameObject.name})] ReflectionProbe rendering");
        }
    }
}
