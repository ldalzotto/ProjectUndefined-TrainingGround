using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class AIAttractiveObjectPersistantManager : AbstractAIAttractiveObjectManager
    {

        public AIAttractiveObjectPersistantManager(NavMeshAgent selfAgent, AiID aiID, PuzzleEventsManager puzzleEventsManager) : base(selfAgent, aiID,puzzleEventsManager)
        {}
        
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
        }
    }
}
