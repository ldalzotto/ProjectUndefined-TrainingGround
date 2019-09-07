using CoreGame;
using GameConfigurationID;
using System;
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
        private InteractiveObjectType AssociatedInteractiveObjectType;
        #endregion

        #region Internal Dependencies
        private CircleFillBarType progressbar;
        #endregion

        private DisarmObjectInherentData disarmObjectInherentConfigurationData;
        private SphereCollider disarmObjectRange;

        private List<AIObjectType> AiThatCanInteract;
        private float elapsedTime;

        public DisarmObjectInherentData DisarmObjectInherentConfigurationData { get => disarmObjectInherentConfigurationData; }

        public void Init(ModelObjectModule ModelObjectModule, InteractiveObjectType AssociatedInteractiveObjectType, DisarmObjectInherentData DisarmObjectInherentConfigurationData)
        {
            this.ModelObjectModule = ModelObjectModule;
            this.AssociatedInteractiveObjectType = AssociatedInteractiveObjectType;
            this.disarmObjectInherentConfigurationData = DisarmObjectInherentConfigurationData;
            this.AiThatCanInteract = new List<AIObjectType>();

            this.disarmObjectRange = this.GetComponent<SphereCollider>();
            this.disarmObjectRange.radius = this.disarmObjectInherentConfigurationData.DisarmInteractionRange;

            this.progressbar = this.GetComponentInChildren<CircleFillBarType>();
            this.progressbar.Init(Camera.main);
            this.progressbar.transform.position = this.GetProgressBarDisplayPosition();
            this.progressbar.gameObject.SetActive(false);

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
        private Vector3 GetProgressBarDisplayPosition()
        {
            return this.transform.position + IRenderBoundRetrievableStatic.GetDisarmProgressBarLocalOffset(this.ModelObjectModule);
        }
        public float GetEffectRange()
        {
            return this.disarmObjectRange.radius;
        }
        #endregion

        public void Tick(float d, float timeAttenuationFactor)
        {        }
        
        public void TickAlways(float d)
        {
            if (this.progressbar.gameObject.activeSelf)
            {
                this.progressbar.Tick(this.GetDisarmPercentage01());
            }
        }

        #region External Event
        public void IncreaseTimeElapsedBy(float increasedTime)
        {
            this.elapsedTime += increasedTime;

            if (this.GetDisarmPercentage01() > 0 && !this.progressbar.gameObject.activeSelf)
            {
                CircleFillBarType.EnableInstace(this.progressbar);
            }
        }
        public void OnDisarmObjectStart()
        {
            this.AssociatedInteractiveObjectType.DisableModule(typeof(GrabObjectModule));
        }

        public void OnDisarmObjectEnd()
        {
            this.AssociatedInteractiveObjectType.EnableModule(typeof(GrabObjectModule), new InteractiveObjectInitializationObject());
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsAI)
            {
                var npcAIManager = AIObjectType.FromCollisionType(collisionType);
                this.AiThatCanInteract.Add(npcAIManager);
                npcAIManager.OnDisarmObjectTriggerEnter(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsAI)
            {
                var npcAIManager = AIObjectType.FromCollisionType(collisionType);
                this.AiThatCanInteract.Remove(npcAIManager);
                npcAIManager.OnDisarmObjectTriggerExit(this);
            }
        }

        public static class DisarmObjectModuleInstancer
        {
            public static void PopuplateFromDefinition(DisarmObjectModule disarmObjectModule, DisarmObjectModuleDefinition disarmObjectModuleDefinition)
            {
                disarmObjectModule.DisarmObjectID = disarmObjectModuleDefinition.DisarmObjectID;
            }
        }
    }
}

