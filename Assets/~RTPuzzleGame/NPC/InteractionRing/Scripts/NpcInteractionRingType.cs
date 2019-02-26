using UnityEngine;

namespace RTPuzzle
{
    public class NpcInteractionRingType : MonoBehaviour
    {
        private Texture2D ringTexture;

        public Texture2D RingTexture { get => ringTexture; }

        public void Init()
        {
            this.ringTexture = new Texture2D(360, 1, TextureFormat.RGB24, false);
            GetComponentInChildren<MeshRenderer>().material.mainTexture = this.ringTexture;
        }
    }
}
