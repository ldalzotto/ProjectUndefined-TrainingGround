using UnityEngine;

namespace RTPuzzle
{
    public class GroundEffectType : MonoBehaviour
    {

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;

        public MeshRenderer MeshRenderer { get => meshRenderer; }
        public MeshFilter MeshFilter { get => meshFilter; }

        public void Init()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
        }

    }

}
