using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIAttractiveObjectLooseManager : AbstractAIAttractiveObjectManager
    {
        public AIAttractiveObjectLooseManager(AIAttractiveObjectComponent associatedAIComponent) : base(associatedAIComponent)
        {
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
            if (this.involvedAttractiveObject != null &&
                attractiveObjectType.AttractiveObjectId == this.involvedAttractiveObject.AttractiveObjectId)
            {
                this.OnStateReset();
            }
        }
    }
}