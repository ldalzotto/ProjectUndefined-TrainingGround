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

        public override Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            if (isAttracted)
            {
                return attractionPosition;
            }
            return null;
        }

        public override void OnTriggerEnter(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType)
        {
            SetAttractedObject(attractivePosition, attractiveObjectType);
        }

        public override void OnTriggerStay(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType)
        {
            SetAttractedObject(attractivePosition, attractiveObjectType);
        }

        private void SetAttractedObject(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType)
        {
            this.attractionPosition = attractivePosition;
            this.involvedAttractiveObject = attractiveObjectType;
            this.SetIsAttracted(true);
        }

        private void SetIsAttracted(bool value)
        {
            if (this.isAttracted && !value)
            {
                this.PuzzleEventsManager.PZ_EVT_AI_Attracted_End(this.involvedAttractiveObject, this.aiID);
            }
            else if (!this.isAttracted && value)
            {
                this.PuzzleEventsManager.PZ_EVT_AI_Attracted_Start(this.involvedAttractiveObject, this.aiID);
            }
            this.isAttracted = value;
        }

        public override void OnTriggerExit()
        {
            this.OnStateReset();
        }

        public override void OnDestinationReached()
        {
            if (!this.HasSensedThePresenceOfAnAttractiveObject())
            {
                this.OnStateReset();
            }
        }

        public override void OnStateReset()
        {
            this.SetIsAttracted(false);
            this.involvedAttractiveObject = null;
        }

    }
}
