namespace InteractiveObjects
{
    public abstract class A_AIInteractiveObject<INIT> : CoreInteractiveObject where INIT : AbstractAIInteractiveObjectInitializerData
    {
        protected INIT AIInteractiveObjectInitializerData;
        protected AIMoveToDestinationSystem AIMoveToDestinationSystem;

        [VE_Nested] protected AnimationObjectSystem AnimationObjectSystem;
        protected LineVisualFeedbackSystem LineVisualFeedbackSystem;

        public A_AIInteractiveObject(IInteractiveGameObject interactiveGameObject, INIT AIInteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            interactiveGameObject.CreateAgent(AIInteractiveObjectInitializerData.AIAgentDefinition);
            interactiveGameObject.CreateLogicCollider(AIInteractiveObjectInitializerData.InteractiveObjectLogicCollider);
            this.AIInteractiveObjectInitializerData = AIInteractiveObjectInitializerData;
            AnimationObjectSystem = new AnimationObjectSystem(this);
            AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, AIInteractiveObjectInitializerData, OnAIDestinationReached);
            LineVisualFeedbackSystem = new LineVisualFeedbackSystem(InteractiveGameObject);
        }

        public override void Tick(float d)
        {
            AnimationObjectSystem.Tick(d);
            LineVisualFeedbackSystem.Tick(d);
        }

        public override void SetAIDestination(AIDestination AIDestination)
        {
            AIMoveToDestinationSystem.SetDestination(AIDestination);
        }

        public override void SetAISpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            AIMoveToDestinationSystem.SetSpeedAttenuationFactor(AIMovementSpeedDefinition);
        }

        public override void OnAnimationObjectSetUnscaledSpeedMagnitude(float unscaledSpeedMagnitude)
        {
            AnimationObjectSystem.SetUnscaledSpeedMagnitude(unscaledSpeedMagnitude);
        }

        public override void AfterTicks()
        {
            AIMoveToDestinationSystem.AfterTicks();
        }

        public override void Destroy()
        {
            LineVisualFeedbackSystem.OnDestroy();
            base.Destroy();
        }
    }
}