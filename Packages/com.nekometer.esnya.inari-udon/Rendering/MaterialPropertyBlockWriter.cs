
using System;
using UdonSharp;
using UnityEngine;
using VRC.Udon;

namespace InariUdon.Rendering
{
    [
        Documentation.ImageAttachments(new [] {
            "https://user-images.githubusercontent.com/2088693/121310202-160c6b00-c93e-11eb-92ec-91583c3f69f0.png",
            "https://user-images.githubusercontent.com/2088693/121310283-2cb2c200-c93e-11eb-9834-c99a901a0f1a.png",
        }),
        UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)
    ]
    public class MaterialPropertyBlockWriter : UdonSharpBehaviour
    {
        [Tooltip("Apply on start")] public bool onStart = true;
        [Tooltip("Apply on enable")] public bool onEnable = false;

        public bool writeColors;
        public Renderer[] colorTargets = { };
        public int[] colorIndices = { };
        public string[] colorNames = { };
        public Color[] colorValues = { };

        public bool writeFloats;
        public Renderer[] floatTargets = { };
        public int[] floatIndices = { };
        public string[] floatNames = { };
        public float[] floatValues = { };

        public bool writeTextures;
        public Renderer[] textureTargets = { };
        public int[] textureIndices = { };
        public string[] textureNames = { };
        public Texture[] textureValues = { };

        public bool writeVectors;
        public Renderer[] vectorTargets = {};
        public int[] vectorIndices = {};
        public string[] vectorNames = {};
        public Vector4[] vectorValues = {};

        private void Start()
        {
            if (onStart) Trigger();
        }

        private void OnEnable()
        {
            if (onEnable) Trigger();
        }

        [Documentation.EventDescription("Apply overrides")]
        public void Trigger()
        {
            var block = new MaterialPropertyBlock();

            if (writeColors)
            {
                for (int i = 0; i < colorTargets.Length; i++)
                {
                    var target = colorTargets[i];
                    var materialIndex = colorIndices[i];
                    target.GetPropertyBlock(block, materialIndex);
                    block.SetColor(colorNames[i], colorValues[i]);
                    target.SetPropertyBlock(block, materialIndex);
                }
            }

            if (writeFloats)
            {
                for (int i = 0; i < floatTargets.Length; i++)
                {
                    var target = floatTargets[i];
                    var materialIndex = floatIndices[i];
                    target.GetPropertyBlock(block, materialIndex);
                    block.SetFloat(floatNames[i], floatValues[i]);
                    target.SetPropertyBlock(block, materialIndex);
                }
            }

            if (writeTextures)
            {
                for (int i = 0; i < textureTargets.Length; i++)
                {
                    var target = textureTargets[i];
                    var materialIndex = textureIndices[i];
                    target.GetPropertyBlock(block, materialIndex);
                    block.SetTexture(textureNames[i], textureValues[i]);
                    target.SetPropertyBlock(block, materialIndex);
                }
            }

            if (writeVectors)
            {
                for (int i = 0; i < vectorTargets.Length; i++)
                {
                    var target = vectorTargets[i];
                    var materialIndex = vectorIndices[i];
                    target.GetPropertyBlock(block, materialIndex);
                    block.SetVector(vectorNames[i], vectorValues[i]);
                    target.SetPropertyBlock(block, materialIndex);
                }
            }
        }

        private void ClearProperties(Renderer[] renderers)
        {
            var properties = new MaterialPropertyBlock();
            foreach (var renderer in renderers)
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++) renderer.SetPropertyBlock(properties, i);
            }
        }

        public void ClearTargetProperties()
        {
            ClearProperties(colorTargets);
            ClearProperties(floatTargets);
            ClearProperties(textureTargets);
            ClearProperties(vectorTargets);
        }
    }

}