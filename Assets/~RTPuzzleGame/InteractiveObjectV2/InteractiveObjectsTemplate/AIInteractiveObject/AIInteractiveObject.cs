using GameConfigurationID;

namespace InteractiveObjectTest
{
    public class AIInteractiveObject : CoreInteractiveObject
    {
        #region State
        private AIAttractiveObjectState AIAttractiveObjectState;
        #endregion

        private AIMoveToDestinationSystem AIMoveToDestinationSystem;
        private LineVisualFeedbackSystem LineVisualFeedbackSystem;

        public AIInteractiveObject(InteractiveGameObject interactiveGameObject, AIInteractiveObjectInitializerData AIInteractiveObjectInitializerData) : base(interactiveGameObject)
        {
            this.AIAttractiveObjectState = new AIAttractiveObjectState(this.OnAIIsJustAttractedByAttractiveObject, this.OnAIIsNoMoreAttractedByAttractiveObject);
            this.InteractiveObjectTag = new InteractiveObjectTag { IsAi = true };
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(interactiveGameObject, AIInteractiveObjectInitializerData);
            this.LineVisualFeedbackSystem = new LineVisualFeedbackSystem(this.InteractiveGameObject);
        }

        public override void Tick(float d, float timeAttenuationFactor)
        {
            this.AIMoveToDestinationSystem.Tick(d, timeAttenuationFactor);
        }

        public override void TickAlways(float d)
        {
            this.LineVisualFeedbackSystem.TickAlways(d);
        }

        public override void TickWhenTimeIsStopped()
        {
            this.AIMoveToDestinationSystem.TickWhenTimeIsStopped();
        }

        public override void Destroy()
        {
            this.LineVisualFeedbackSystem.OnDestroy();
            base.Destroy();
        }

        public override void OnOtherAttractiveObjectJustIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
            this.AIAttractiveObjectState.IsAttractedByAttractiveObject = true;
            this.SetAIDestination(new AIDestination { WorldPosition = OtherInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition });
            this.LineVisualFeedbackSystem.CreateLineFollowing(DottedLineID.ATTRACTIVE_OBJECT, OtherInteractiveObject);
        }

        public override void OnOtherAttractiveObjectNoMoreIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
            if (this.AIAttractiveObjectState.IsAttractedByAttractiveObject)
            {
                this.AIMoveToDestinationSystem.ClearPath();
                this.AIAttractiveObjectState.IsAttractedByAttractiveObject = false;
            }
            this.LineVisualFeedbackSystem.DestroyLine(OtherInteractiveObject);
        }
        
        public override void SetAIDestination(AIDestination AIDestination) { this.AIMoveToDestinationSystem.SetDestination(AIDestination); }
    }
}
