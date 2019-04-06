using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "AIAttractiveObjectComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIAttractiveObjectComponent", order = 1)]
    public class AIAttractiveObjectComponent : AbstractAIComponent
    {
        protected override Type abstractManagerType => typeof(AbstractAIAttractiveObjectManager);
    }



    public abstract class AbstractAIAttractiveObjectManager
    {

        protected NavMeshAgent selfAgent;
        protected AiID aiID;

        #region State
        protected bool isAttracted;
        #endregion
        protected AttractiveObjectType involvedAttractiveObject;

        public abstract Vector3? TickComponent();

        #region External Events
        public abstract void OnTriggerEnter(Collider collider, CollisionType collisionType);
        public abstract void OnTriggerStay(Collider collider, CollisionType collisionType);
        public abstract void OnTriggerExit(Collider collider, CollisionType collisionType);
        public abstract void OnDestinationReached();
        public abstract void OnStateReset();

        public void ClearAttractedObject()
        {
            this.involvedAttractiveObject = null;
        }

        public void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy)
        {
            if (this.IsDestructedAttractiveObjectEqualsToCurrent(attractiveObjectToDestroy))
            {
                this.OnDestinationReached();
            }
        }
        #endregion

        #region Logical Conditions
        private bool IsDestructedAttractiveObjectEqualsToCurrent(AttractiveObjectType attractiveObjectToDestroy)
        {
            return (this.involvedAttractiveObject != null &&
                attractiveObjectToDestroy.GetInstanceID() == this.involvedAttractiveObject.GetInstanceID());
        }
        public bool IsInfluencedByAttractiveObject()
        {
            return this.isAttracted || this.HasSensedThePresenceOfAnAttractiveObject();
        }
        public bool HasSensedThePresenceOfAnAttractiveObject()
        {
            return (this.involvedAttractiveObject != null && this.involvedAttractiveObject.IsInRangeOf(this.selfAgent.transform.position));
        }
        #endregion
    }

}
