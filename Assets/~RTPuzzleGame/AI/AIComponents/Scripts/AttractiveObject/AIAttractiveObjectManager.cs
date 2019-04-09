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

        public override void OnTriggerEnter(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType)//  Collider collider, CollisionType collisionType)
        {
            SetAttractedObject(attractivePosition, attractiveObjectType);
        }

        public override void OnTriggerStay(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType)//(Collider collider, CollisionType collisionType)
        {
            if (!this.IsInfluencedByAttractiveObject())
            {
                SetAttractedObject(attractivePosition, attractiveObjectType);
            }
        }

        private void SetAttractedObject(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType)//CollisionType collisionType)
        {
            this.attractionPosition = attractivePosition;
            this.involvedAttractiveObject = attractiveObjectType;
            this.SetIsAttracted(true);
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

        public override void OnTriggerExit()
        {
            OnDestinationReached();
        }

        public override void OnDestinationReached()
        {
            this.OnStateReset();
        }

        public override void OnStateReset()
        {
            this.SetIsAttracted(false);
            this.involvedAttractiveObject = null;
        }
    }
}
