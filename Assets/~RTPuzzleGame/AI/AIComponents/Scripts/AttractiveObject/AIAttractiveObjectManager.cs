using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class AIAttractiveObjectManager : AbstractAIAttractiveObjectManager
    {

        #region External Events
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion
        
        public AIAttractiveObjectManager(NavMeshAgent selfAgent, AiID aiID, PuzzleEventsManager puzzleEventsManager)
        {
            this.selfAgent = selfAgent;
            this.aiID = aiID;
            this.PuzzleEventsManager = puzzleEventsManager;
        }

        private Vector3? attractionPosition;

        public override Vector3? TickComponent()
        {
            if (isAttracted)
            {
                return attractionPosition;
            }
            return null;
        }

        public override void OnTriggerEnter(Collider collider, CollisionType collisionType)
        {
            SetAttractedObject(collisionType);
        }

        public override void OnTriggerStay(Collider collider, CollisionType collisionType)
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

        public override void OnTriggerExit(Collider collider, CollisionType collisionType)
        {
            OnDestinationReached();
        }

        public override void OnDestinationReached()
        {
            this.SetIsAttracted(false);
            this.involvedAttractiveObject = null;
        }
       

    }
}
