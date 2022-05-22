#pragma warning disable IDE0051,IDE1006

using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace InariUdon.Sync
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncedObjectMultiplexer : UdonSharpBehaviour
    {
        public int initialIndex = 0;
        public GameObject[] targets = {};
        [UdonSynced, FieldChangeCallback(nameof(Index))] private int _index;
        public int Index {
            set {
                if (value < 0) value = targets.Length - 1;
                if (value >= targets.Length) value = 0;

                for (int i = 0; i < targets.Length; i++)
                {
                    var target = targets[i];
                    if (target == null) continue;
                    target.SetActive(i == value);
                }
                _index = value;
            }
            get => _index;
        }

        private void Start()
        {
            Index = initialIndex;
        }

        public void _SetIndex(int value)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Index = value;
            RequestSerialization();
        }
        public void _Increment() => _SetIndex(Index + 1);
        public void _Decrement() => _SetIndex(Index - 1);

        public override void Interact() => _Increment();
    }
}
