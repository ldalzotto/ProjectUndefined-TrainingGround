using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class AIAttractiveObjectManager
    {

        private NavMeshAgent selfAgent;

        public AIAttractiveObjectManager(NavMeshAgent selfAgent)
        {
            this.selfAgent = selfAgent;
        }

        #region State
        private bool isAttracted;
        #endregion
        private AttractiveObjectType involvedAttractiveObject;
        private Vector3? attractionPosition;
        
        public Vector3? TickComponent()
        {
            if (isAttracted)
            {
                return attractionPosition;
            }
            return null;
        }

        public void OnTriggerEnter(Collider collider, CollisionType collisionType)
        {
            SetAttractedObject(collisionType);
        }

        public void OnTriggerStay(Collider collider, CollisionType collisionType)
        {
            if (collisionType.IsRTAttractiveObject)
            {
                SetAttractedObject(collisionType);
            }
        }

        private void SetAttractedObject(CollisionType collisionType)
        {
            this.isAttracted = true;
            this.attractionPosition = collisionType.transform.position;
            this.involvedAttractiveObject = AttractiveObjectType.GetAttractiveObjectFromCollisionType(collisionType);
        }

        internal void OnTriggerExit(Collider collider, CollisionType collisionType)
        {
            OnDestinationReached();
        }

        public void OnDestinationReached()
        {
            this.isAttracted = false;
        }

        public void ClearAttractedObject()
        {
            this.involvedAttractiveObject = null;
        }

        #region Logical Conditions
        public bool IsDestructedAttractiveObjectEqualsToCurrent(AttractiveObjectType attractiveObjectToDestroy)
        {
            return (this.involvedAttractiveObject != null &&
                attractiveObjectToDestroy.GetInstanceID() == this.involvedAttractiveObject.GetInstanceID());
        }
        public bool IsInfluencedByAttractiveObject()
        {
            return this.isAttracted || (this.involvedAttractiveObject != null && this.involvedAttractiveObject.IsInRangeOf(this.selfAgent.transform.position));
        }
        #endregion

    }
}
