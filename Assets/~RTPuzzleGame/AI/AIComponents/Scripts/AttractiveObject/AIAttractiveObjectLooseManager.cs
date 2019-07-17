using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIAttractiveObjectLooseManager : AbstractAIAttractiveObjectManager
    {

        public void Init(NavMeshAgent selfAgent, AiID aiID, PuzzleEventsManager PuzzleEventsManager)
        {
            this.BaseInit(selfAgent, aiID, PuzzleEventsManager);
        }

        public override void ComponentTriggerEnter(Vector3 attractivePosition, AttractiveObjectTypeModule attractiveObjectType)
        {
            SetAttractedObject(attractivePosition, attractiveObjectType);
        }

        public override void ComponentTriggerStay(Vector3 attractivePosition, AttractiveObjectTypeModule attractiveObjectType)
        {
            SetAttractedObject(attractivePosition, attractiveObjectType);
        }

        public override void ComponentTriggerExit(AttractiveObjectTypeModule attractiveObjectType)
        {
            if (this.involvedAttractiveObject != null &&
                attractiveObjectType.AttractiveObjectId == this.involvedAttractiveObject.AttractiveObjectId)
            {
                this.OnStateReset();
            }
        }
    }
}