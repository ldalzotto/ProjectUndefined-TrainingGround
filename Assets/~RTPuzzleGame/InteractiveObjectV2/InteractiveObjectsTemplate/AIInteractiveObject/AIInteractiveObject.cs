using GameConfigurationID;

namespace InteractiveObjectTest
{

    public class AIInteractiveObject : CoreInteractiveObject
    {
        #region State
        private AIPatrollingState AIPatrollingState;
        private AIAttractiveObjectState AIAttractiveObjectState;
        private AIDisarmObjectState AIDisarmObjectState;
        #endregion

        private AnimationObjectSystem AnimationObjectSystem;
        private AIMoveToDestinationSystem AIMoveToDestinationSystem;
        private AIPatrolSystem AIPatrolSystem;
        private SightObjectSystem SightObjectSystem;
        private LineVisualFeedbackSystem LineVisualFeedbackSystem;

        public AIInteractiveObject(InteractiveGameObject interactiveGameObject, AIInteractiveObjectInitializerData AIInteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            this.AIAttractiveObjectState = new AIAttractiveObjectState(this.OnAIIsJustAttractedByAttractiveObject, this.OnAIIsNoMoreAttractedByAttractiveObject);
            this.AIDisarmObjectState = new AIDisarmObjectState(this.OnAIIsJustDisarmingObject, this.OnAIIsNoMoreJustDisarmingObject);
            this.InteractiveObjectTag = new InteractiveObjectTag { IsAi = true };

            this.AnimationObjectSystem = new AnimationObjectSystem(this);
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, AIInteractiveObjectInitializerData, this.OnAIDestinationReached);

            if (AIInteractiveObjectInitializerData.IsPatrolling)
            {
                this.AIPatrollingState.isPatrolling = true;
                this.AIPatrolSystem = new AIPatrolSystem(this, AIInteractiveObjectInitializerData.AIPatrolSystemDefinition);
            }

            if (AIInteractiveObjectInitializerData.HasSight)
            {
                this.SightObjectSystem = new SightObjectSystem(this, AIInteractiveObjectInitializerData.SightObjectSystemDefinition, new InteractiveObjectTagStruct { IsAttractiveObject = 1 },
                    this.OnSightObjectSystemJustIntersected, this.OnSightObjectSystemIntersectedNothing, this.OnSightObjectSystemNoMoreIntersected);
            }
            this.LineVisualFeedbackSystem = new LineVisualFeedbackSystem(this.InteractiveGameObject);
        }

        public override void Tick(float d, float timeAttenuationFactor)
        {
            if (this.AIPatrollingState.isPatrolling)
            {
                this.AIPatrolSystem.Tick(d, timeAttenuationFactor);
            }

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

        public override void OnAIDestinationReached()
        {
            this.AIPatrolSystem.OnAIDestinationReached();
        }

        public override void OnAnimationObjectSetUnscaledSpeedMagnitude(AnimationObjectSetUnscaledSpeedMagnitudeEvent AnimationObjectSetUnscaledSpeedMagnitudeEvent)
        {
            this.AnimationObjectSystem.SetUnscaledSpeedMagnitude(AnimationObjectSetUnscaledSpeedMagnitudeEvent);
        }

        #region Attractive Object
        public override void OnOtherAttractiveObjectJustIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
            if (!this.AIDisarmObjectState.IsDisarming)
            {
                this.AIAttractiveObjectState.SetIsAttractedByAttractiveObject(true, OtherInteractiveObject);
                this.SetAIDestination(new AIDestination { WorldPosition = OtherInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition });
                this.LineVisualFeedbackSystem.CreateLineFollowing(DottedLineID.ATTRACTIVE_OBJECT, OtherInteractiveObject);
            }
        }

        public override void OnOtherAttractiveObjectIntersectedNothing(CoreInteractiveObject OtherInteractiveObject)
        {
            if (!this.AIDisarmObjectState.IsDisarming && !this.AIAttractiveObjectState.IsAttractedByAttractiveObject)
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

        public override void OnAIIsNoMoreAttractedByAttractiveObject(CoreInteractiveObject AttractedInteractiveObject)
        {
            this.LineVisualFeedbackSystem.DestroyLine(AttractedInteractiveObject);
        }
        #endregion

        public override void SetAIDestination(AIDestination AIDestination)
        {
            this.AIMoveToDestinationSystem.SetDestination(AIDestination);
        }

        public override void OnOtherDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
            this.AIAttractiveObjectState.SetIsAttractedByAttractiveObject(false, OtherInteractiveObject);
            this.AIMoveToDestinationSystem.ClearPath();
            this.AIDisarmObjectState.IsDisarming = true;
        }

        public override void OnOtherDisarmobjectTriggerExit(CoreInteractiveObject OtherInteractiveObject)
        {
            this.AIDisarmObjectState.IsDisarming = false;
        }

        protected override void OnSightObjectSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            if (!this.AIDisarmObjectState.IsDisarming)
            {
                if (IntersectedInteractiveObject.InteractiveObjectTag.IsAttractiveObject)
                {
                    this.AIAttractiveObjectState.SetIsAttractedByAttractiveObject(true, IntersectedInteractiveObject);
                }
                this.SetAIDestination(new AIDestination { WorldPosition = IntersectedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition });
            }
        }

        protected override void OnSightObjectSystemIntersectedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.OnSightObjectSystemJustIntersected(IntersectedInteractiveObject);
        }
    }
}
