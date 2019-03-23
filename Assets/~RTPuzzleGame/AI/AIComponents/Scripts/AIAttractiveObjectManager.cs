using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class AIAttractiveObjectManager
    {

        #region External Events
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private NavMeshAgent selfAgent;
        private AiID aiID;

        public AIAttractiveObjectManager(NavMeshAgent selfAgent, AiID aiID, PuzzleEventsManager puzzleEventsManager)
        {
            this.selfAgent = selfAgent;
            this.aiID = aiID;
            this.PuzzleEventsManager = puzzleEventsManager;
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
            if (collisionType.IsRTAttractiveObject && !this.IsInfluencedByAttractiveObject())
            {
                SetAttractedObject(collisionType);
            }
        }

        private void SetAttractedObject(CollisionType collisionType)
        {
            this.SetIsAttracted(true);
            this.attractionPosition = collisionType.transform.position;
            this.involvedAttractiveObject = AttractiveObjectType.GetAttractiveObjectFromCollisionType(collisionType);
        }

        private void SetIsAttracted(bool value)
        {
            if (this.isAttracted && !value)
            {
                this.PuzzleEventsManager.OnAIAttractedEnd(this.involvedAttractiveObject, this.aiID);
            }
            else if (!this.isAttracted && value)
            {
                this.PuzzleEventsManager.OnAIAttractedStart(this.involvedAttractiveObject, this.aiID);
            }
            this.isAttracted = value;
        }

        internal void OnTriggerExit(Collider collider, CollisionType collisionType)
        {
            OnDestinationReached();
        }

        public void OnDestinationReached()
        {
            this.SetIsAttracted(false);
            this.involvedAttractiveObject = null;
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
            return this.isAttracted || this.HasSensedThePresenceOfAnAttractiveObject();
        }
        public bool HasSensedThePresenceOfAnAttractiveObject()
        {
            return (this.involvedAttractiveObject != null && this.involvedAttractiveObject.IsInRangeOf(this.selfAgent.transform.position));
        }
        #endregion

    }
}
