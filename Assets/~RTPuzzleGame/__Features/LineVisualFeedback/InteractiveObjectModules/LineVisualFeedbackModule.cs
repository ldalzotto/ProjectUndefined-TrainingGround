using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class LineVisualFeedbackModule : InteractiveObjectModule, ILineVisualFeedbackEvent
    {

        #region External Dependencies
        private InteractiveObjectContainer InteractiveObjectContainer;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval, 
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            this.PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;

            //position calculation
            this.positionOffsetFromNPC = IRenderBoundRetrievableStatic.GetLineRenderPointLocalOffset(IInteractiveObjectTypeDataRetrieval.GetModelObjectModule());
        }

        private Vector3 positionOffsetFromNPC;
        private Vector3 targetWorldPositionOffset;
        private DottedLine AttractiveObjectDottedLine;
        private ModelObjectModule AttractiveObjectModelObjectModule;


        public void TickAlways(float d)
        {
            if (this.AttractiveObjectDottedLine != null)
            {
                this.AttractiveObjectDottedLine.Tick(d, this.transform.position + this.positionOffsetFromNPC, AttractiveObjectModelObjectModule.transform.position + this.targetWorldPositionOffset);
            }
        }

        #region ILineVisualFeedbackEvent
        public void CreateLine(DottedLineID DottedLineID, ModelObjectModule TargetModelObjectModule)
        {
            if (this.AttractiveObjectDottedLine == null)
            {
                this.AttractiveObjectDottedLine = DottedLine.CreateInstance(DottedLineID, this.PuzzleGameConfigurationManager);
            }
            this.AttractiveObjectModelObjectModule = TargetModelObjectModule;
            this.targetWorldPositionOffset = IRenderBoundRetrievableStatic.GetLineRenderPointLocalOffset(TargetModelObjectModule);
        }

        public void DestroyLine()
        {
            if (this.AttractiveObjectDottedLine != null)
            {
                this.AttractiveObjectDottedLine.DestroyInstance();
            }
        }
        #endregion
    }
}
