using GameConfigurationID;

namespace InteractiveObjectTest
{

    public class AIInteractiveObjectTest : A_AIInteractiveObject<AIInteractiveObjectTestInitializerData>
    {
        #region State
        [VE_Nested]
        private AIPatrollingState AIPatrollingState;
        [VE_Nested]
        private AIAttractiveObjectState AIAttractiveObjectState;
        [VE_Nested]
        private AIDisarmObjectState AIDisarmObjectState;
        #endregion

        private AIPatrolSystem AIPatrolSystem;
        [VE_Nested]
        private SightObjectSystem SightObjectSystem;
        private LocalCutscenePlayerSystem LocalCutscenePlayerSystem;

        public AIInteractiveObjectTest(InteractiveGameObject interactiveGameObject, AIInteractiveObjectTestInitializerData AIInteractiveObjectInitializerData) : base(interactiveGameObject, AIInteractiveObjectInitializerData)
        {
            this.AIPatrollingState = new AIPatrollingState();
            this.AIAttractiveObjectState = new AIAttractiveObjectState(new BoolVariable(false, this.OnAIIsJustAttractedByAttractiveObject, this.OnAIIsNoMoreAttractedByAttractiveObject));
            this.AIDisarmObjectState = new AIDisarmObjectState(new BoolVariable(false, this.OnAIIsJustDisarmingObject, this.OnAIIsNoMoreJustDisarmingObject));
            this.interactiveObjectTag = new InteractiveObjectTag { IsAi = true };

            this.AIPatrolSystem = new AIPatrolSystem(this, AIInteractiveObjectInitializerData.AIPatrolSystemDefinition);

            this.SightObjectSystem = new SightObjectSystem(this, AIInteractiveObjectInitializerData.SightObjectSystemDefinition, new InteractiveObjectTagStruct { IsAttractiveObject = 1 },
                      this.OnSightObjectSystemJustIntersected, this.OnSightObjectSystemIntersectedNothing, this.OnSightObjectSystemNoMoreIntersected);
            this.LocalCutscenePlayerSystem = new LocalCutscenePlayerSystem();
        }

        public override void Tick(float d, float timeAttenuationFactor)
        {

            if (!this.AIDisarmObjectState.IsDisarming.GetValue() && !this.AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue())
            {
                this.AIPatrollingState.isPatrolling = true;
            }

            if (this.AIPatrollingState.isPatrolling)
            {
                this.AIPatrolSystem.Tick(d, timeAttenuationFactor);
            }

            this.AIMoveToDestinationSystem.Tick(d, timeAttenuationFactor);
        }

        public override void TickAlways(float d)
        {
            this.LocalCutscenePlayerSystem.TickAlways(d);
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

        public override void SetAIDestination(AIDestination AIDestination)
        {
            this.AIMoveToDestinationSystem.SetDestination(AIDestination);
        }

        public override void SetAISpeedAttenuationFactor(AIMovementDefinitions.AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            this.AIMoveToDestinationSystem.SetSpeedAttenuationFactor(AIMovementSpeedDefinition);
        }

        #region Attractive Object
        public override void OnOtherAttractiveObjectJustIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
            if (!this.AIDisarmObjectState.IsDisarming.GetValue())
            {
                SwitchToAttractedState(OtherInteractiveObject);
            }
        }

        private void SwitchToAttractedState(CoreInteractiveObject OtherInteractiveObject)
        {
            this.AIAttractiveObjectState.SetIsAttractedByAttractiveObject(true, OtherInteractiveObject);
            this.AIPatrollingState.isPatrolling = false;
            this.SetAISpeedAttenuationFactor(this.AIInteractiveObjectInitializerData.AISpeedWhenAttracted);
            this.SetAIDestination(new AIDestination { WorldPosition = OtherInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition });
        }

        public override void OnOtherAttractiveObjectIntersectedNothing(CoreInteractiveObject OtherInteractiveObject)
        {
            if (!this.AIDisarmObjectState.IsDisarming.GetValue() && !this.AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue())
            {
                this.SwitchToAttractedState(OtherInteractiveObject);
            }
        }

        public override void OnOtherAttractiveObjectNoMoreIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
            if (this.AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue())
            {
                this.AIMoveToDestinationSystem.ClearPath();
                this.AIAttractiveObjectState.SetIsAttractedByAttractiveObject(false, OtherInteractiveObject);
            }
            this.LineVisualFeedbackSystem.DestroyLine(OtherInteractiveObject);
        }

        public override void OnAIIsJustAttractedByAttractiveObject()
        {
            this.LineVisualFeedbackSystem.CreateLineFollowing(DottedLineID.ATTRACTIVE_OBJECT, this.AIAttractiveObjectState.AttractedInteractiveObject);
        }

        public override void OnAIIsNoMoreAttractedByAttractiveObject()
        {
            this.LineVisualFeedbackSystem.DestroyLine(this.AIAttractiveObjectState.AttractedInteractiveObject);
        }
        #endregion
        
        public override void OnOtherDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
            this.AIAttractiveObjectState.SetIsAttractedByAttractiveObject(false, OtherInteractiveObject);
            this.AIMoveToDestinationSystem.ClearPath();
            this.AIDisarmObjectState.IsDisarming.SetValue(true);
        }

        public override void OnOtherDisarmobjectTriggerExit(CoreInteractiveObject OtherInteractiveObject)
        {
            this.AIDisarmObjectState.IsDisarming.SetValue(false);
        }

        public override void OnAIIsJustDisarmingObject()
        {
            this.LocalCutscenePlayerSystem.PlayCutscene(this.AIInteractiveObjectInitializerData.DisarmObjectAnimation.GetSequencedActions(this));
        }

        public override void OnAIIsNoMoreJustDisarmingObject()
        {
            this.LocalCutscenePlayerSystem.KillCurrentCutscene();
        }

        protected override void OnSightObjectSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            if (!this.AIDisarmObjectState.IsDisarming.GetValue())
            {
                this.AIPatrollingState.isPatrolling = false;
                this.SwitchToAttractedState(IntersectedInteractiveObject);
                if (!this.AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue())
                {
                    this.LineVisualFeedbackSystem.CreateLineFollowing(DottedLineID.ATTRACTIVE_OBJECT, IntersectedInteractiveObject);
                }
            }
        }

        protected override void OnSightObjectSystemIntersectedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.OnSightObjectSystemJustIntersected(IntersectedInteractiveObject);
        }
    }
}