using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using InteractiveObjectTest;

namespace RTPuzzle
{
    public class AIMoveToActionV2 : SequencedAction, IActionAbortedOnDestinationReached
    {
        private bool destinationReached;
        private CoreInteractiveObject InteractiveObject;
        private TransformStruct WorldPoint;
        
        public AIMoveToActionV2(CoreInteractiveObject InteractiveObject, TransformStruct WorldPoint, List<SequencedAction> nextActions) : base(nextActions)
        {
            this.InteractiveObject = InteractiveObject;
            this.WorldPoint = WorldPoint;
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
            this.InteractiveObject.SetAIDestination(new AIDestination
            {
                WorldPosition = this.WorldPoint.WorldPosition,
                Rotation = this.WorldPoint.WorldRotation
            });
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
