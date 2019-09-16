using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class FovInteractionRingType : MonoBehaviour
    {

        public Color AvailableColor;
        public Color UnavailableColor;

        private Texture2D ringTexture;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;

        #region Data Retrieval
        public Texture2D RingTexture { get => ringTexture; }
        public MeshRenderer MeshRenderer { get => meshRenderer; }
        public MeshFilter MeshFilter { get => meshFilter; }
        public Bounds GetBoundingRect()
        {
            return this.meshRenderer.bounds;
        }
        #endregion

        #region Logical Conditions
        public bool IsActive()
        {
            return this.gameObject.activeSelf;
        }
        #endregion

        public void Init()
        {
            this.ringTexture = ProceduralTextureHelper.CreateSliceTexture(360);
            this.meshRenderer = GetComponentInChildren<MeshRenderer>();
            this.meshFilter = GetComponentInChildren<MeshFilter>();
        }

        #region External Events
        public void OnActivate()
        {
            this.gameObject.SetActive(true);
        }

        public void OnDeactivate()
        {
            this.gameObject.SetActive(false);
        }
        #endregion

        #region data Retrieval
        public Bounds GetBounds()
        {
            return this.meshRenderer.bounds;
        }
        #endregion
    }
}
