using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class AIAttractiveObjectPersistantManager : AbstractAIAttractiveObjectManager
    {
        public void Init(NavMeshAgent selfAgent, AiID aiID, PuzzleEventsManager puzzleEventsManager)
        {
            this.BaseInit(selfAgent, aiID, puzzleEventsManager);
        }

        public override void ComponentTriggerEnter(Vector3 attractivePosition, AttractiveObjectModule attractiveObjectType)
        {
            SetAttractedObject(attractivePosition, attractiveObjectType);
        }

        public override void ComponentTriggerStay(Vector3 attractivePosition, AttractiveObjectModule attractiveObjectType)
        {
            SetAttractedObject(attractivePosition, attractiveObjectType);
        }

        public override void ComponentTriggerExit(AttractiveObjectModule attractiveObjectType)
        {
        }
    }
}
