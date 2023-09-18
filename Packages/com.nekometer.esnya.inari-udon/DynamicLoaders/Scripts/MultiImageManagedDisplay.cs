
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace InariUdon.DynamicDownloaders
{
    /// <summary>
    /// Display downloaded images from MultiImageManager. Requires MultiImageManager in the parent GameObject or set it to manager property.
    /// </summary>
    [DefaultExecutionOrder(100)] // After MultiImageManager
    [RequireComponent(typeof(Renderer))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MultiImageManagedDisplay : UdonSharpBehaviour
    {
        /// <summary>
        /// Manager of downloading images. If not specified, it will be searched in the parent GameObject.
        /// </summary>
        public MultiImageManager manager;


        /// <summary>
        /// Interval in seconds between to show next image.
        /// </summary>
        public float interval = 15;


        /// <summary>
        /// Shuffle order of urls. Based on UTC time and position of transform.
        /// Generally results in the same order of urls across clients.
        /// </summary>
        public bool shuffle = true;

        /// <summary>
        /// Scale of randomizer. How much the position of transform affects the shuffle order.
        /// </summary>
        public float randoizerScale = 0.25f;

        /// <summary>
        /// Index of material to be applied.
        /// </summary>
        public int materialIndex = 0;

        /// <summary>
        /// Property name of texture to be applied.
        /// </summary>
        public string propertyName = "_MainTex";

        private Texture[] textures;
        private bool[] errored;
        private bool[] requests;
        private int[] indices;
        private bool initialized = false;
        private float nextUpdateTime = 0;
        private float utcOffset = 0;
        private new Renderer renderer;
        private MaterialPropertyBlock properties;

        private void Start()
        {
            if (manager == null) manager = GetComponentInParent<MultiImageManager>();
        }

        private void Initialize()
        {
            textures = manager.textures;
            errored = manager.errored;
            requests = manager.requests;

            utcOffset = (DateTime.UtcNow.Second - Time.time) % textures.Length;

            properties = new MaterialPropertyBlock();
            renderer = GetComponent<Renderer>();
            renderer.GetPropertyBlock(properties, materialIndex);

            initialized = true;
        }

        private int GetIndex()
        {
            var n = Mathf.RoundToInt((Time.time - utcOffset) / interval) % textures.Length;

            if (shuffle)
            {
                var position = transform.position * randoizerScale;
                n = (n + Mathf.FloorToInt((Mathf.PerlinNoise(position.x, position.z) + Mathf.PerlinNoise((float)n / textures.Length, position.y)) * textures.Length)) % textures.Length;
            }

            if (errored[n])
            {
                for (var i = 0; i < errored.Length; i++)
                {
                    if (!errored[(i + n) % errored.Length]) return (i + n) % errored.Length;
                }
            }

            return n;
        }


        private void ApplyTexture(Texture texture)
        {
            properties.SetTexture(propertyName, texture);
            renderer.SetPropertyBlock(properties, materialIndex);
        }

        private void Schedule()
        {
            nextUpdateTime = Time.time + interval;
        }

        private void OnRenderObject()
        {
            if (Time.time < nextUpdateTime) return;

            if (!initialized) Initialize();

            var index = GetIndex();
            var texture = textures[index];
            if (texture)
            {
                ApplyTexture(texture);
                Schedule();
                return;
            }

            if (errored[index])
            {
                Schedule();
                return;
            }

            if (!requests[index])
            {
                requests[index] = true;
            }
        }
    }
}
