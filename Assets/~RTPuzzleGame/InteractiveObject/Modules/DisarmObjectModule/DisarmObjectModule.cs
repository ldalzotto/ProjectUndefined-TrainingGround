using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class DisarmObjectModule : InteractiveObjectModule
    {
        [CustomEnum()]
        public DisarmObjectID DisarmObjectID;

        #region Module Dependencies
        private ModelObjectModule ModelObjectModule;
        #endregion

        private DisarmObjectInherentData disarmObjectInherentConfigurationData;

        private List<NPCAIManager> AiThatCanInteract;
        private float elapsedTime;

        public DisarmObjectInherentData DisarmObjectInherentConfigurationData { get => disarmObjectInherentConfigurationData; }

        public static InteractiveObjectType Instanciate(Vector3 worldPosition, DisarmObjectInherentData DisarmObjectInherentData)
        {
            InteractiveObjectType createdDisarmObject = MonoBehaviour.Instantiate(DisarmObjectInherentData.AssociatedInteractiveObjectType);
            createdDisarmObject.Init(new InteractiveObjectInitializationObject() { DisarmObjectInherentData = DisarmObjectInherentData });
            createdDisarmObject.transform.position = worldPosition;
            return createdDisarmObject;
        }

        public void Init(ModelObjectModule ModelObjectModule, DisarmObjectInherentData DisarmObjectInherentConfigurationData)
        {
            this.ModelObjectModule = ModelObjectModule;
            this.disarmObjectInherentConfigurationData = DisarmObjectInherentConfigurationData;
            this.AiThatCanInteract = new List<NPCAIManager>();

            this.GetComponent<SphereCollider>().radius = this.disarmObjectInherentConfigurationData.DisarmInteractionRange;
            this.elapsedTime = 0f;
        }

        #region Logical Conditions
        public bool IsAskingToBeDestroyed()
        {
            return (this.elapsedTime >= this.disarmObjectInherentConfigurationData.DisarmTime);
        }
        #endregion

        #region Data Retrieval
        public float GetDisarmPercentage01()
        {
            return this.elapsedTime / this.disarmObjectInherentConfigurationData.DisarmTime;
        }
        public Vector3 GetProgressBarDisplayPosition()
        {
            return this.transform.position + IRenderBoundRetrievableStatic.GetDisarmProgressBarLocalOffset(this.ModelObjectModule);
        }
        #endregion

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

