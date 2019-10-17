using CoreGame;
using InteractiveObjects;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class AIMoveToActionV2 : SequencedAction, IActionAbortedOnDestinationReached
    {
        private bool destinationReached;
        private CoreInteractiveObject InteractiveObject;
        private TransformStruct WorldPoint;
        private AIMovementSpeedDefinition AIMovementSpeed;

        public AIMoveToActionV2(CoreInteractiveObject InteractiveObject, TransformStruct WorldPoint, AIMovementSpeedDefinition AIMovementSpeed, List<SequencedAction> nextActions) : base(nextActions)
        {
            this.InteractiveObject = InteractiveObject;
            this.WorldPoint = WorldPoint;
            this.AIMovementSpeed = AIMovementSpeed;
        }

        public override void AfterFinishedEventProcessed() { }

        public override bool ComputeFinishedConditions()
        {
            return this.destinationReached;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.destinationReached = false;
        }

        public override void Tick(float d)
        {
            if (!this.destinationReached)
            {
                this.InteractiveObject.SetAIDestination(new AIDestination
                {
                    WorldPosition = this.WorldPoint.WorldPosition,
                    Rotation = this.WorldPoint.WorldRotation
                });
                this.InteractiveObject.SetAISpeedAttenuationFactor(this.AIMovementSpeed);
            }
        }

        public void OnDestinationReached()
        {
            this.Interupt();
        }

        public override void Interupt()
        {
            this.destinationReached = true;
        }
    }

}
