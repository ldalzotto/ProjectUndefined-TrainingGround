using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class LineVisualFeedbackManager
    {

        #region External Dependencies
        private InteractiveObjectContainer InteractiveObjectContainer;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private DottedLineContainer DottedLineContainer;
        #endregion

        public LineVisualFeedbackManager(AIObjectType nPCAIManagerRef)
        {
            this.InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            this.PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            this.DottedLineContainer = PuzzleGameSingletonInstances.DottedLineContainer;

            //position calculation
            this.positionOffsetFromNPC = IRenderBoundRetrievableStatic.GetLineRenderPointLocalOffset(nPCAIManagerRef);
        }

        private Vector3 positionOffsetFromNPC;
        private Vector3 targetWorldPositionOffset;
        private DottedLine AttractiveObjectDottedLine;
        private IAttractiveObjectModuleDataRetriever IAttractiveObjectModuleDataRetriever;


        public void Tick(float d, Vector3 npcAIBoundsCenterWorldPosition)
        {
            if (this.AttractiveObjectDottedLine != null)
            {
                this.AttractiveObjectDottedLine.Tick(d, npcAIBoundsCenterWorldPosition + this.positionOffsetFromNPC, IAttractiveObjectModuleDataRetriever.GetTransform().position + this.targetWorldPositionOffset);
            }
        }

        #region External Events
        public void OnAttractiveObjectStart(IAttractiveObjectModuleDataRetriever IAttractiveObjectModuleDataRetriever)
        {
            if (this.AttractiveObjectDottedLine == null)
            {
                this.AttractiveObjectDottedLine = DottedLine.CreateInstance(DottedLineID.ATTRACTIVE_OBJECT, this.PuzzleGameConfigurationManager, this.DottedLineContainer);
            }
            this.IAttractiveObjectModuleDataRetriever = IAttractiveObjectModuleDataRetriever;
            this.targetWorldPositionOffset = IRenderBoundRetrievableStatic.GetLineRenderPointLocalOffset(IAttractiveObjectModuleDataRetriever.GetModelObjectModule());
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
