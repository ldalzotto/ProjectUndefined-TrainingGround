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

        #region Sight Event
        protected void OnSightObjectSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.AIPatrollingState.isPatrolling = false;
            this.SetAISpeedAttenuationFactor(AIMovementDefinitions.AIMovementSpeedDefinition.RUN);
            this.SetAIDestination(new AIDestination { WorldPosition = IntersectedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition });
        }
        protected void OnSightObjectSystemIntersectedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            this.OnSightObjectSystemJustIntersected(IntersectedInteractiveObject);
        }
        protected void OnSightObjectSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            if (this.SightObjectSystem.CurrentlyIntersectedInteractiveObjects.Count > 0)
            {
                this.OnSightObjectSystemJustIntersected(this.SightObjectSystem.CurrentlyIntersectedInteractiveObjects[0]);
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
