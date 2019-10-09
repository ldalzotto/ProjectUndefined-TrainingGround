using UnityEngine;

namespace RTPuzzle
{
    public class AIMoveTowardPlayerManager : AbstractAIMoveTowardPlayerManager
    {
        private AIObjectTypeSpeedSetter AIObjectTypeSpeedSetter;

        #region State
        protected bool playerInSight;
        protected CollisionType currentTarget;
        private Vector3? currentDestination;
        #endregion

        public AIMoveTowardPlayerManager(AIMoveTowardPlayerComponent associatedAIComponent) : base(associatedAIComponent)
        { }

        public override void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            this.AIObjectTypeSpeedSetter = AIBheaviorBuildInputData.AIObjectTypeSpeedSetter();
        }

        #region External Events
        public override void OnDestinationReached()
        {
            this.OnStateReset();
        }

        public override void OnStateReset()
        {
            this.playerInSight = false;
            this.currentDestination = null;
        }

        #endregion
        
        public override void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            this.AIObjectTypeSpeedSetter.SetSpeedAttenuationFactor(this.AssociatedAIComponent.AISpeed);
            if (this.playerInSight && this.currentTarget != null)
            {
                this.currentDestination = this.currentTarget.GetAssociatedCollider().transform.position;
            }
            NPCAIDestinationContext.TargetPosition = this.currentDestination;
        }

        public override bool IsManagerEnabled()
        {
            return this.playerInSight || (this.currentDestination != null && this.currentDestination.HasValue);
        }

        public override void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
        }

#if UNITY_EDITOR
        #region Test data retrieval
        public override CollisionType GetCurrentTarget() { return this.currentTarget; }
        #endregion
#endif
    }
}
