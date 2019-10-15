namespace InteractiveObjectTest
{
    [SceneHandleDraw]
    public class AggressiveAIObject : A_AIInteractiveObject<AggressiveObjectInitializerData>
    {
        [VE_Nested]
        private AIPatrollingState AIPatrollingState;

        [DrawNested]
        private SightObjectSystem SightObjectSystem;

        private AIPatrolSystem AIPatrolSystem;

        public AggressiveAIObject(InteractiveGameObject interactiveGameObject, AggressiveObjectInitializerData AIInteractiveObjectInitializerData) : base(interactiveGameObject, AIInteractiveObjectInitializerData)
        {
            this.interactiveObjectTag = new InteractiveObjectTag { IsAi = true };
            this.AIPatrollingState = new AIPatrollingState();
            this.AIPatrollingState.isPatrolling = true;
            this.AIPatrolSystem = new AIPatrolSystem(this, AIInteractiveObjectInitializerData.AIPatrolSystemDefinition);
            this.SightObjectSystem = new SightObjectSystem(this, AIInteractiveObjectInitializerData.SightObjectSystemDefinition, new InteractiveObjectTagStruct { IsPlayer = 1 },
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


        #region Sight Event
        protected override void OnSightObjectSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.AIPatrollingState.isPatrolling = false;
            this.SetAIDestination(new AIDestination { WorldPosition = IntersectedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition });
        }
        protected override void OnSightObjectSystemIntersectedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.AIPatrollingState.isPatrolling = false;
            this.SetAIDestination(new AIDestination { WorldPosition = IntersectedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition });
        }
        protected override void OnSightObjectSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            if (this.SightObjectSystem.CurrentlyIntersectedInteractiveObjects.Count > 0)
            {
                this.AIPatrollingState.isPatrolling = false;
                this.SetAIDestination(new AIDestination { WorldPosition = this.SightObjectSystem.CurrentlyIntersectedInteractiveObjects[0].InteractiveGameObject.GetTransform().WorldPosition });
            }
            else
            {
                this.AIPatrollingState.isPatrolling = true;
            }
        }
        #endregion

        public override void OnAIDestinationReached()
        {
            this.AIPatrolSystem.OnAIDestinationReached();
        }
    }
}
