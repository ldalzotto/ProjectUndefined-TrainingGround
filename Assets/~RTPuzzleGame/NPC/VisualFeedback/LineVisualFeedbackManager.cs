using UnityEngine;

namespace RTPuzzle
{
    public class LineVisualFeedbackManager
    {

        #region External Dependencies
        private AttractiveObjectsContainerManager AttractiveObjectsContainerManager;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private DottedLineContainer DottedLineContainer;
        #endregion

        public LineVisualFeedbackManager(NPCAIManager nPCAIManagerRef)
        {
            this.AttractiveObjectsContainerManager = GameObject.FindObjectOfType<AttractiveObjectsContainerManager>();
            this.PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            this.DottedLineContainer = GameObject.FindObjectOfType<DottedLineContainer>();

            //position calculation
            this.positionOffsetFromNPC = IRenderBoundRetrievableStatic.GetLineRenderPointLocalOffset(nPCAIManagerRef);
        }

        private Vector3 positionOffsetFromNPC;
        private Vector3 targetWorldPositionOffset;
        private DottedLine AttractiveObjectDottedLine;
        private AttractiveObjectId AttractiveObjectId;


        public void Tick(float d, Vector3 npcAIBoundsCenterWorldPosition)
        {
            if (this.AttractiveObjectDottedLine != null)
            {
                var attractiveObject = this.AttractiveObjectsContainerManager.GetAttractiveObjectType(AttractiveObjectId);
                this.AttractiveObjectDottedLine.Tick(d, npcAIBoundsCenterWorldPosition + this.positionOffsetFromNPC, attractiveObject.transform.position + this.targetWorldPositionOffset);
            }
        }

        #region External Events
        public void OnAttractiveObjectStart(AttractiveObjectId attractiveObjectId)
        {
            if (this.AttractiveObjectDottedLine == null)
            {
                this.AttractiveObjectDottedLine = DottedLine.CreateInstance(DottedLineID.ATTRACTIVE_OBJECT, this.PuzzleGameConfigurationManager, this.DottedLineContainer);
            }
            this.AttractiveObjectId = attractiveObjectId;
            var attractiveObject = this.AttractiveObjectsContainerManager.GetAttractiveObjectType(AttractiveObjectId);
            this.targetWorldPositionOffset = IRenderBoundRetrievableStatic.GetLineRenderPointLocalOffset(attractiveObject);
        }
        public void OnAttractiveObjectEnd()
        {
            if (this.AttractiveObjectDottedLine != null)
            {
                this.AttractiveObjectDottedLine.DestroyInstance();
            }
        }
        #endregion

    }
}
