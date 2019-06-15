using UnityEngine;

namespace RTPuzzle
{
    public class NpcInteractionRingManager
    {

        private const string INTERACTION_RING_OBJECT_NAME = "InteractionRing";

        #region External Renferences
        private NPCAIManager npcAiManagerRef;
        private NpcInteractionRingContainer NpcInteractionRingContainer;
        #endregion

        private NpcInteractionRingType npcInteractionRingType;
        private Vector3 ringPositionOffset;

        #region Data Retrieval
        public Vector3 RingPositionOffset { get => ringPositionOffset; }
        public Vector3 GetRingPosition()
        {
            return this.npcInteractionRingType.transform.position;
        }
        #endregion

        #region Logical Conditions
        public float GetInteractionRingHeight()
        {
            if (this.npcInteractionRingType.IsActive())
            {
                return this.npcInteractionRingType.GetBounds().size.y;
            }
            return 0f;
        }
        #endregion

        public NpcInteractionRingManager(NPCAIManager npcAiManagerRef)
        {
            #region External Dependencies
            this.NpcInteractionRingContainer = GameObject.FindObjectOfType<NpcInteractionRingContainer>();
            #endregion

            ComputePositionOffset(npcAiManagerRef);

            this.npcAiManagerRef = npcAiManagerRef;
            var npcInteractionRingContainer = GameObject.FindObjectOfType<NpcInteractionRingContainer>();

            npcInteractionRingType = GameObject.Instantiate(PrefabContainer.Instance.NpcInteractionRingPrefab, npcInteractionRingContainer.transform);
            npcInteractionRingType.transform.rotation = Quaternion.Euler(Vector3.forward);
            npcInteractionRingType.Init();
            this.NpcInteractionRingContainer.OnNpcInteractionRingCreated(npcInteractionRingType);
        }

        #region External Events
        public void OnFOVChanged(FOV newFOV)
        {

            var colors = npcInteractionRingType.RingTexture.GetPixels();
            for (var i = 0; i < colors.Length; i++)
            {
                colors[i] = npcInteractionRingType.UnavailableColor;
            }

            foreach (var fovSlice in newFOV.FovSlices)
            {
                ComputeColorsPixel(fovSlice.BeginAngleIncluded, fovSlice.EndAngleExcluded, ref colors);
            }
            npcInteractionRingType.RingTexture.SetPixels(colors);
            npcInteractionRingType.RingTexture.Apply(false);

            //display or hide ring
            if (newFOV.GetSumOfAvailableAngleDeg() < 360f)
            {
                this.NpcInteractionRingContainer.OnNpcInteractionRingSetUnder360(this.npcInteractionRingType);
            }
            else
            {
                this.NpcInteractionRingContainer.OnNpcInteractionRingSetTo360(this.npcInteractionRingType);
            }
        }
        #endregion

        public void Tick(float d)
        {
            npcInteractionRingType.transform.position = this.npcAiManagerRef.transform.position + ringPositionOffset;
        }

        private void ComputeColorsPixel(float beginAngle, float endAngle, ref Color[] colors)
        {
            var beginAngleInt = Mathf.RoundToInt(beginAngle);
            var endAngleInt = Mathf.RoundToInt(endAngle);

            for (var i = beginAngleInt; i < endAngleInt; i++)
            {
                colors[i] = npcInteractionRingType.AvailableColor;
            }
        }

        private void ComputePositionOffset(NPCAIManager npcAiManagerRef)
        {
            ringPositionOffset = new Vector3(0, npcAiManagerRef.GetAverageModelBoundLocalSpace().Bounds.max.y, 0);
        }
    }
}
