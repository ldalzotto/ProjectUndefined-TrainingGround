using GameConfigurationID;

namespace InteractiveObjectTest
{

    public class AIInteractiveObjectTest : A_AIInteractiveObject<AIInteractiveObjectTestInitializerData>
    {
        #region State
        private AIPatrollingState AIPatrollingState;
        private AIAttractiveObjectState AIAttractiveObjectState;
        private AIDisarmObjectState AIDisarmObjectState;
        #endregion

        private AIPatrolSystem AIPatrolSystem;
        private SightObjectSystem SightObjectSystem;

        public AIInteractiveObjectTest(InteractiveGameObject interactiveGameObject, AIInteractiveObjectTestInitializerData AIInteractiveObjectInitializerData) : base(interactiveGameObject, AIInteractiveObjectInitializerData)
        {
            this.AIAttractiveObjectState = new AIAttractiveObjectState(this.OnAIIsJustAttractedByAttractiveObject, this.OnAIIsNoMoreAttractedByAttractiveObject);
            this.AIDisarmObjectState = new AIDisarmObjectState(this.OnAIIsJustDisarmingObject, this.OnAIIsNoMoreJustDisarmingObject);
            this.InteractiveObjectTag = new InteractiveObjectTag { IsAi = true };

            this.AIPatrollingState.isPatrolling = true;
            this.AIPatrolSystem = new AIPatrolSystem(this, AIInteractiveObjectInitializerData.AIPatrolSystemDefinition);

            this.SightObjectSystem = new SightObjectSystem(this, AIInteractiveObjectInitializerData.SightObjectSystemDefinition, new InteractiveObjectTagStruct { IsAttractiveObject = 1 },
                      this.OnSightObjectSystemJustIntersected, this.OnSightObjectSystemIntersectedNothing, this.OnSightObjectSystemNoMoreIntersected);
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
            base.TickAlways(d);
        }

        public override void TickWhenTimeIsStopped()
        {
            base.TickWhenTimeIsStopped();
        }

        public override void AfterTicks()
        {
            base.AfterTicks();
        }

        public override void Destroy()
        {
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