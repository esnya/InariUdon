
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace EsnyaFactory.InariUdon
{
    public class ReflectionProbe : UdonSharpBehaviour
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
