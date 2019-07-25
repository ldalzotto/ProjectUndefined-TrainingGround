using UnityEngine;

namespace RTPuzzle
{
    public class AIMoveTowardPlayerManager : AbstractAIManager, InterfaceAIManager
    {
        private AISightVision AISightVision;

        #region State
        protected bool playerInSight;
        protected ColliderWithCollisionType currentTarget;
        #endregion

        public void Init(AISightVision AISightVision)
        {
            this.AISightVision = AISightVision;
        }

        #region External Events
        public virtual void OnDestinationReached()
        {
            this.OnStateReset();
        }

        public virtual void OnStateReset()
        {
            this.playerInSight = false;
        }

        #endregion

        public bool OnSightInRangeEnter(SightInRangeEnterAIBehaviorEvent sightInRangeEnterAIBehaviorEvent)
        {
            if (sightInRangeEnterAIBehaviorEvent.ColliderWithCollisionType.collisionType.IsPlayer)
            {
                this.playerInSight = true;
                this.currentTarget = sightInRangeEnterAIBehaviorEvent.ColliderWithCollisionType;
                return true;
            }
            return false;
        }

        public void OnSightInRangeExit(SightInRangeExitAIBehaviorEvent sightInRangeExitAIBehaviorEvent)
        {
            if (sightInRangeExitAIBehaviorEvent.ColliderWithCollisionType.collisionType.IsPlayer)
            {
                this.playerInSight = false;
                this.currentTarget = null;
            }
        }

        public virtual Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            if (this.playerInSight && this.currentTarget != null)
            {
                return this.currentTarget.collider.transform.position;
            }
            return null;
        }

        public bool IsManagerEnabled()
        {
            return this.playerInSight;
        }

        public void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
        }

#if UNITY_EDITOR
        #region Test data retrieval
        public ColliderWithCollisionType GetCurrentTarget() { return this.currentTarget; }
        #endregion
#endif
    }
}
