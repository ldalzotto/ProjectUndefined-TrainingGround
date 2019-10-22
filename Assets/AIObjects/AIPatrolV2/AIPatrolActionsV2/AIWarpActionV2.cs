using System.Collections.Generic;
using CoreGame;
using InteractiveObjects;

namespace AIObjects
{
    public class AIWarpActionV2 : SequencedAction
    {
        private CoreInteractiveObject InteractiveObject;
        private TransformStruct WorldPoint;

        public AIWarpActionV2(CoreInteractiveObject InteractiveObject, TransformStruct WorldPoint,
            List<SequencedAction> nextActions) : base(nextActions)
        {
            this.InteractiveObject = InteractiveObject;
            this.WorldPoint = WorldPoint;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.InteractiveObject.InteractiveGameObject.Agent.Warp(this.WorldPoint.WorldPosition);
            this.InteractiveObject.InteractiveGameObject.Agent.transform.rotation = this.WorldPoint.WorldRotation;
        }

        public override void Tick(float d)
        {
        }
    }
}