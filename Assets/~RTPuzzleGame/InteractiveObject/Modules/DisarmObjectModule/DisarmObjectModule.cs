using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class DisarmObjectModule : InteractiveObjectModule
    {
        [CustomEnum()]
        public DisarmObjectID DisarmObjectID;

        private DisarmObjectInherentData DisarmObjectInherentConfigurationData;

        private List<NPCAIManager> AiThatCanInteract;

        public void Init(DisarmObjectInherentData DisarmObjectInherentConfigurationData)
        {
            this.DisarmObjectInherentConfigurationData = DisarmObjectInherentConfigurationData;
            this.AiThatCanInteract = new List<NPCAIManager>();

            this.GetComponent<SphereCollider>().radius = this.DisarmObjectInherentConfigurationData.DisarmInteractionRange;
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsAI)
            {
                this.AiThatCanInteract.Add(NPCAIManager.FromCollisionType(collisionType));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsAI)
            {
                this.AiThatCanInteract.Remove(NPCAIManager.FromCollisionType(collisionType));
            }
        }
    }
}

