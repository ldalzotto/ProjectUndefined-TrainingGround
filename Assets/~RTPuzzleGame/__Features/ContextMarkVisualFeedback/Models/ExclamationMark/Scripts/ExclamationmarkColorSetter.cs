using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class ExclamationmarkColorSetter : MonoBehaviour
    {

        public Color exclamationColor;

        private MeshRenderer meshRenderer;
        private MaterialPropertyBlock propertyBlock;

        void Start()
        {
            this.meshRenderer = GetComponentInChildren<MeshRenderer>();
            this.propertyBlock = new MaterialPropertyBlock();
            this.propertyBlock.SetColor("_Color", exclamationColor);
            this.meshRenderer.SetPropertyBlock(this.propertyBlock);
        }

        private void Update()
        {
            this.propertyBlock.SetColor("_Color", exclamationColor);
            this.meshRenderer.SetPropertyBlock(this.propertyBlock);
        }

    }
}
