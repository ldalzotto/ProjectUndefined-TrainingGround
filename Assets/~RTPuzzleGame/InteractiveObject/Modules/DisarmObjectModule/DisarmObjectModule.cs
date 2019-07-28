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
        private float elapsedTime;

        public void Init(DisarmObjectInherentData DisarmObjectInherentConfigurationData)
        {
            this.DisarmObjectInherentConfigurationData = DisarmObjectInherentConfigurationData;
            this.AiThatCanInteract = new List<NPCAIManager>();

            this.GetComponent<SphereCollider>().radius = this.DisarmObjectInherentConfigurationData.DisarmInteractionRange;
            this.elapsedTime = 0f;
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
        }

        #region External Event
        public void IncreaseTimeElapsedBy(float increasedTime)
        {
            this.elapsedTime += increasedTime;
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsAI)
            {
                var npcAIManager = NPCAIManager.FromCollisionType(collisionType);
                this.AiThatCanInteract.Add(npcAIManager);
                npcAIManager.OnDisarmObjectTriggerEnter(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsAI)
            {
                var npcAIManager = NPCAIManager.FromCollisionType(collisionType);
                this.AiThatCanInteract.Remove(npcAIManager);
                npcAIManager.OnDisarmObjectTriggerExit(this);
            }
        }
    }
}

