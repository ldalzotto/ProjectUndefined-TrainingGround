using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIAttractiveObjectLooseManager : AbstractAIAttractiveObjectManager
    {
        public AIAttractiveObjectLooseManager(NavMeshAgent selfAgent, AiID aiID, PuzzleEventsManager PuzzleEventsManager) : base(selfAgent, aiID, PuzzleEventsManager)
        {
        }

        public override void OnTriggerEnter(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType)
        {
            SetAttractedObject(attractivePosition, attractiveObjectType);
        }

        public override void OnTriggerStay(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType)
        {
            SetAttractedObject(attractivePosition, attractiveObjectType);
        }

        public override void OnTriggerExit(AttractiveObjectType attractiveObjectType)
        {
            if (this.involvedAttractiveObject != null &&
                attractiveObjectType.AttractiveObjectId == this.involvedAttractiveObject.AttractiveObjectId)
            {
                this.OnStateReset();
            }
        }
    }
}