using CoreGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class FovModule : InteractiveObjectModule, IFovManagerCalcuation, IFovModuleDataRetrieval
    {

        private const string INTERACTION_RING_OBJECT_NAME = "InteractionRing";

        #region Module Dependencies
        private IRenderBoundRetrievable IRenderBoundRetrievable;
        #endregion

        #region External Renferences
        private FovInteractionRingContainer FovInteractionRingContainer;
        #endregion

        private FovManager fovManager;
        private Vector3 ringPositionOffset;
        private FovInteractionRingType npcInteractionRingType;

        #region Data Retrieval
        public FovManager FovManager { get => fovManager; }
        #endregion

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.IRenderBoundRetrievable = IInteractiveObjectTypeDataRetrieval.GetModelObjectModule();

            #region External Dependencies
            this.FovInteractionRingContainer = PuzzleGameSingletonInstances.NpcInteractionRingContainer;
            #endregion

            this.npcInteractionRingType = GetComponentInChildren<FovInteractionRingType>();
            npcInteractionRingType.transform.rotation = Quaternion.Euler(Vector3.forward);
            npcInteractionRingType.Init();

            this.FovInteractionRingContainer.OnNpcInteractionRingCreated(npcInteractionRingType);

            this.fovManager = new FovManager(interactiveObjectInitializationObject.ParentAIObjectTypeReference.GetAgent(), this.OnFOVChanged);

            this.ringPositionOffset = new Vector3(0, this.IRenderBoundRetrievable.GetAverageModelBoundLocalSpace().Bounds.max.y, 0);
        }

        public void TickAlways(float d)
        {
            npcInteractionRingType.transform.position = this.transform.position + ringPositionOffset;
            npcInteractionRingType.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        private void OnFOVChanged(FOV newFOV)
        {
            ProceduralTextureHelper.SetTextureSliceColor(npcInteractionRingType.RingTexture, newFOV.FovSlices, npcInteractionRingType.AvailableColor, npcInteractionRingType.UnavailableColor);

            //display or hide ring
            if (newFOV.GetSumOfAvailableAngleDeg() < 360f)
            {
                this.FovInteractionRingContainer.OnNpcInteractionRingSetUnder360(this.npcInteractionRingType);
            }
            else
            {
                this.FovInteractionRingContainer.OnNpcInteractionRingSetTo360(this.npcInteractionRingType);
            }
        }

        internal void GizmoTick()
        {
            this.fovManager.GizmoTick();
        }

        #region IFovManagerCalcuation
        public NavMeshHit[] NavMeshRaycastSample(int sampleNB, Transform sourceTransform, float raySampleDistance)
        {
            return this.fovManager.NavMeshRaycastSample(sampleNB, sourceTransform, raySampleDistance);
        }

        public NavMeshHit[] NavMeshRaycastEndOfRanges(Transform sourceTransform, float raySampleDistance)
        {
            return this.fovManager.NavMeshRaycastEndOfRanges(sourceTransform, raySampleDistance);
        }

        public List<StartEndSlice> IntersectFOV_FromEscapeDirection(Vector3 from, Vector3 to, float escapeSemiAngle)
        {
            return this.fovManager.IntersectFOV_FromEscapeDirection(from, to, escapeSemiAngle);
        }

        public List<StartEndSlice> IntersectFOV(float beginAngle, float endAngle)
        {
            return this.fovManager.IntersectFOV(beginAngle, endAngle);
        }

        public void ResetFOV()
        {
            this.fovManager.ResetFOV();
        }

        public float GetFOVAngleSum()
        {
            return this.fovManager.GetFOVAngleSum();
        }
        #endregion

        #region IFovModuleDataRetrieval
        public Vector3 GetRingPositionOffset()
        {
            return this.ringPositionOffset;
        }

        public float GetInteractionRingHeight()
        {
            if (this.npcInteractionRingType.IsActive())
            {
                return this.npcInteractionRingType.GetBounds().size.y;
            }
            return 0f;
        }
        #endregion
    }
}
