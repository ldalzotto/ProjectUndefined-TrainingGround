namespace InteractiveObjects
{
    [SceneHandleDraw]
    public class AggressiveAIObject : A_AIInteractiveObject<AggressiveObjectInitializerData>
    {
        [VE_Nested] private AIPatrollingState AIPatrollingState;

        private AIPatrolSystem AIPatrolSystem;

        [DrawNested] private SightObjectSystem SightObjectSystem;

        public AggressiveAIObject(IInteractiveGameObject interactiveGameObject, AggressiveObjectInitializerData AIInteractiveObjectInitializerData) : base(interactiveGameObject, AIInteractiveObjectInitializerData)
        {
            interactiveObjectTag = new InteractiveObjectTag {IsAi = true};
            AIPatrollingState = new AIPatrollingState();
            AIPatrollingState.isPatrolling = true;
            AIPatrolSystem = new AIPatrolSystem(this, AIInteractiveObjectInitializerData.AIPatrolSystemDefinition);
            SightObjectSystem = new SightObjectSystem(this, AIInteractiveObjectInitializerData.SightObjectSystemDefinition, new InteractiveObjectTagStruct {IsPlayer = 1},
                OnSightObjectSystemJustIntersected, OnSightObjectSystemIntersectedNothing, OnSightObjectSystemNoMoreIntersected);

            AfterConstructor();
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            if (AIPatrollingState.isPatrolling) AIPatrolSystem.Tick(d);

            AIMoveToDestinationSystem.Tick(d);
        }

        public override void OnAIDestinationReached()
        {
            AIPatrolSystem.OnAIDestinationReached();
        }

        #region Sight Event

        protected void OnSightObjectSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            AIPatrollingState.isPatrolling = false;
            SetAISpeedAttenuationFactor(AIMovementSpeedDefinition.RUN);
            SetAIDestination(new AIDestination {WorldPosition = IntersectedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition});
        }

        protected void OnSightObjectSystemIntersectedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            OnSightObjectSystemJustIntersected(IntersectedInteractiveObject);
        }

        protected void OnSightObjectSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            if (SightObjectSystem.CurrentlyIntersectedInteractiveObjects.Count > 0)
                OnSightObjectSystemJustIntersected(SightObjectSystem.CurrentlyIntersectedInteractiveObjects[0]);
            else
                AIPatrollingState.isPatrolling = true;
        }

        #endregion
    }
}