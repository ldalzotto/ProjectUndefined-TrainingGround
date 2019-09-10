using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class NpcInteractionRingManager
    {

        private const string INTERACTION_RING_OBJECT_NAME = "InteractionRing";

        #region External Renferences
        private AIObjectType npcAiManagerRef;
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

        public NpcInteractionRingManager(AIObjectType npcAiManagerRef)
        {
            #region External Dependencies
            this.NpcInteractionRingContainer = PuzzleGameSingletonInstances.NpcInteractionRingContainer;
            #endregion

            ComputePositionOffset(npcAiManagerRef);

            this.npcAiManagerRef = npcAiManagerRef;

            npcInteractionRingType = GameObject.Instantiate(PrefabContainer.Instance.NpcInteractionRingPrefab, this.NpcInteractionRingContainer.transform);
            npcInteractionRingType.transform.rotation = Quaternion.Euler(Vector3.forward);
            npcInteractionRingType.Init();
            this.NpcInteractionRingContainer.OnNpcInteractionRingCreated(npcInteractionRingType);
        }

        #region External Events
        public void OnFOVChanged(FOV newFOV)
        {
            ProceduralTextureHelper.SetTextureSliceColor(npcInteractionRingType.RingTexture, newFOV.FovSlices, npcInteractionRingType.AvailableColor, npcInteractionRingType.UnavailableColor);

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

        private void ComputePositionOffset(AIObjectType npcAiManagerRef)
        {
            ringPositionOffset = new Vector3(0, npcAiManagerRef.GetAverageModelBoundLocalSpace().Bounds.max.y, 0);
        }
    }
}
