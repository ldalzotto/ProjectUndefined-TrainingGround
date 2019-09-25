using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class LineVisualFeedbackModule : InteractiveObjectModule, ILineVisualFeedbackEvent
    {

        #region External Dependencies
        private InteractiveObjectContainer InteractiveObjectContainer;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        #region Module Dependencies
        private ModelObjectModule ModelObjectModule;
        #endregion

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            this.PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;

            this.ModelObjectModule = IInteractiveObjectTypeDataRetrieval.GetModelObjectModule();

            //position calculation
            this.positionOffsetFromNPC = IRenderBoundRetrievableStatic.GetLineRenderPointLocalOffset(this.ModelObjectModule);

            this.sourceTriggeringInstanceIds = new List<int>();
            this.linePositionings = new List<ILinePositioning>();
            this.lines = new List<DottedLine>();
        }

        private Vector3 positionOffsetFromNPC;

        private List<int> sourceTriggeringInstanceIds;
        private List<ILinePositioning> linePositionings;
        private List<DottedLine> lines;

        public void TickAlways(float d)
        {
            for(var i = 0;i<this.lines.Count; i++)
            {
                var startPosition = this.transform.position + this.positionOffsetFromNPC;
                this.lines[i].Tick(d, startPosition, this.linePositionings[i].GetEndPosition(startPosition));
            }
        }

        #region ILineVisualFeedbackEvent
        public void CreateLineFollowingModelObject(DottedLineID DottedLineID, ModelObjectModule TargetModelObjectModule, MonoBehaviour sourceTriggeringObject)
        {
            this.sourceTriggeringInstanceIds.Add(sourceTriggeringObject.GetInstanceID());
            this.lines.Add(DottedLine.CreateInstance(DottedLineID, this.PuzzleGameConfigurationManager));
            this.linePositionings.Add(new LineFollowModelObjectPositioning(TargetModelObjectModule));
        }

        public void CreateLineDirectionPositioning(DottedLineID DottedLineID, MonoBehaviour sourceTriggeringObject)
        {
            this.sourceTriggeringInstanceIds.Add(sourceTriggeringObject.GetInstanceID());
            this.lines.Add(DottedLine.CreateInstance(DottedLineID, this.PuzzleGameConfigurationManager));
            this.linePositionings.Add(new LineDirectionPositioning(this.ModelObjectModule, sourceTriggeringObject));
        }

        public void DestroyLine(MonoBehaviour sourceTriggeringObject)
        {
            var index = this.sourceTriggeringInstanceIds.IndexOf(sourceTriggeringObject.GetInstanceID());
            if(index >= 0)
            {
                this.sourceTriggeringInstanceIds.RemoveAt(index);
                this.linePositionings.RemoveAt(index);
                this.lines[index].DestroyInstance();
                this.lines.RemoveAt(index);
            }
        }
        #endregion
    }
}
