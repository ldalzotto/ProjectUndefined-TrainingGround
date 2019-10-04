using GameConfigurationID;

namespace InteractiveObjectTest
{
    public class AIInteractiveObject : CoreInteractiveObject
    {
        #region State
        private AIAttractiveObjectState AIAttractiveObjectState;
        private AIDisarmObjectState AIDisarmObjectState;
        #endregion

        private AnimationObjectSystem AnimationObjectSystem;
        private AIMoveToDestinationSystem AIMoveToDestinationSystem;
        private LineVisualFeedbackSystem LineVisualFeedbackSystem;

        public AIInteractiveObject(InteractiveGameObject interactiveGameObject, AIInteractiveObjectInitializerData AIInteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            this.AIAttractiveObjectState = new AIAttractiveObjectState(this.OnAIIsJustAttractedByAttractiveObject, this.OnAIIsNoMoreAttractedByAttractiveObject);
            this.AIDisarmObjectState = new AIDisarmObjectState(this.OnAIIsJustDisarmingObject, this.OnAIIsNoMoreJustDisarmingObject);
            this.InteractiveObjectTag = new InteractiveObjectTag { IsAi = true };

            this.AnimationObjectSystem = new AnimationObjectSystem(this);
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, AIInteractiveObjectInitializerData);
            this.LineVisualFeedbackSystem = new LineVisualFeedbackSystem(this.InteractiveGameObject);
        }

        public override void Tick(float d, float timeAttenuationFactor)
        {
            this.AIMoveToDestinationSystem.Tick(d, timeAttenuationFactor);
        }

        public override void TickAlways(float d)
        {
            this.AnimationObjectSystem.TickAlways(d);
            this.LineVisualFeedbackSystem.TickAlways(d);
        }

        public override void TickWhenTimeIsStopped()
        {
            this.AIMoveToDestinationSystem.TickWhenTimeIsStopped();
        }

        public override void AfterTicks()
        {
            this.AIMoveToDestinationSystem.AfterTicks();
        }

        public override void Destroy()
        {
            this.LineVisualFeedbackSystem.OnDestroy();
            base.Destroy();
        }

        public override void OnAnimationObjectSetUnscaledSpeedMagnitude(AnimationObjectSetUnscaledSpeedMagnitudeEvent AnimationObjectSetUnscaledSpeedMagnitudeEvent)
        {
            this.AnimationObjectSystem.SetUnscaledSpeedMagnitude(AnimationObjectSetUnscaledSpeedMagnitudeEvent);
        }

        public override void OnOtherAttractiveObjectJustIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
            if (!this.AIDisarmObjectState.IsDisarming)
            {
                this.AIAttractiveObjectState.SetIsAttractedByAttractiveObject(true, OtherInteractiveObject);
                this.SetAIDestination(new AIDestination { WorldPosition = OtherInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition });
                this.LineVisualFeedbackSystem.CreateLineFollowing(DottedLineID.ATTRACTIVE_OBJECT, OtherInteractiveObject);
            }
        }

        public override void OnOtherAttractiveObjectNoMoreIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
            if (this.AIAttractiveObjectState.IsAttractedByAttractiveObject)
            {
                this.AIMoveToDestinationSystem.ClearPath();
                this.AIAttractiveObjectState.SetIsAttractedByAttractiveObject(false, OtherInteractiveObject);
            }
            this.LineVisualFeedbackSystem.DestroyLine(OtherInteractiveObject);
        }

        protected override void OnAttractiveSystemInterestedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            if (!this.AIAttractiveObjectState.IsAttractedByAttractiveObject)
            {
                this.OnOtherAttractiveObjectNoMoreIntersected(IntersectedInteractiveObject);
            }
        }

        public override void OnAIIsNoMoreAttractedByAttractiveObject(CoreInteractiveObject AttractedInteractiveObject)
        {
            this.LineVisualFeedbackSystem.DestroyLine(AttractedInteractiveObject);
        }

        public override void SetAIDestination(AIDestination AIDestination) { this.AIMoveToDestinationSystem.SetDestination(AIDestination); }

        public override void OnOtherDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject, out bool success)
        {
            this.AIAttractiveObjectState.SetIsAttractedByAttractiveObject(false, OtherInteractiveObject);
            this.AIMoveToDestinationSystem.ClearPath();
            this.AIDisarmObjectState.IsDisarming = true;
            success = this.AIDisarmObjectState.IsDisarming;
        }
    }
}
