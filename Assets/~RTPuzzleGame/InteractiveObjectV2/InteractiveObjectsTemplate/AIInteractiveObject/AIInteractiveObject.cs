using GameConfigurationID;

namespace InteractiveObjectTest
{
    public abstract class A_AIInteractiveObject<INIT> : CoreInteractiveObject where INIT : A_AIInteractiveObjectInitializerData
    {
        protected AnimationObjectSystem AnimationObjectSystem;
        protected AIMoveToDestinationSystem AIMoveToDestinationSystem;
        protected LineVisualFeedbackSystem LineVisualFeedbackSystem;

        public A_AIInteractiveObject(InteractiveGameObject interactiveGameObject, INIT AIInteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            this.AnimationObjectSystem = new AnimationObjectSystem(this);
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, AIInteractiveObjectInitializerData, this.OnAIDestinationReached);
            this.LineVisualFeedbackSystem = new LineVisualFeedbackSystem(this.InteractiveGameObject);
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
    }

}
