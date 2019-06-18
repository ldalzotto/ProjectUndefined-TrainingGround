using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class AttractiveObjectsContainerManager : MonoBehaviour
    {
        #region External Dependencies
        private NPCAIManagerContainer NPCAIManagerContainer;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        private AttractiveObjectsContainer AttractiveObjectsContainer;

        public void Init()
        {
            this.NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            this.PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            AttractiveObjectsContainer = new AttractiveObjectsContainer(this);
        }

        public void InitStaticInitials()
        {
            //Initialize all static initial attractiveObjects
            foreach (var attractiveObjectType in GameObject.FindObjectsOfType<AttractiveObjectType>())
            {
                attractiveObjectType.Init(this.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[attractiveObjectType.AttractiveObjectId]);
            }
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            AttractiveObjectsContainer.Tick(d, timeAttenuationFactor);
        }

        #region External Events
        public void OnAttractiveObjectActionExecuted(RaycastHit attractiveObjectWorldPositionHit, AttractiveObjectId attractiveObjectId)
        {
            var attractiveObjectInherentConfigurationData = this.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[attractiveObjectId];
            var instanciatedAttractiveObject = AttractiveObjectType.Instanciate(attractiveObjectWorldPositionHit.point, transform, attractiveObjectInherentConfigurationData);
            //TODO make the rotation relative to the player
            instanciatedAttractiveObject.transform.LookAt(instanciatedAttractiveObject.transform.position + Vector3.forward);
        }

        public void OnAttracteObjectCreated(AttractiveObjectType attractiveObjectType)
        {
            Debug.Log("Attractive object created");
            AttractiveObjectsContainer.AddAttractiveObject(attractiveObjectType);
        }
        #endregion

        #region Internal Events
        public void OnAttractiveObjectDestroy(AttractiveObjectType attractiveObjectToDestroy)
        {
            AttractiveObjectsContainer.OnAttractiveObjectDestroy(attractiveObjectToDestroy);
            this.NPCAIManagerContainer.OnAttractiveObjectDestroyed(attractiveObjectToDestroy);
            MonoBehaviour.Destroy(attractiveObjectToDestroy.gameObject);
        }
        #endregion

        #region Data Retrieval
        public AttractiveObjectType GetAttractiveObjectType(AttractiveObjectId attractiveObjectTypeID)
        {
            return this.AttractiveObjectsContainer.GetAttractiveObjectType(attractiveObjectTypeID);
        }
        #endregion
    }

    class AttractiveObjectsContainer
    {
        private Dictionary<AttractiveObjectId, AttractiveObjectType> attractiveObjects;
        private AttractiveObjectsContainerManager AttractiveObjectsContainerManager;

        public AttractiveObjectsContainer(AttractiveObjectsContainerManager AttractiveObjectsContainerManager)
        {
            this.AttractiveObjectsContainerManager = AttractiveObjectsContainerManager;
            this.attractiveObjects = new Dictionary<AttractiveObjectId, AttractiveObjectType>();
        }

        public void AddAttractiveObject(AttractiveObjectType attractiveObject)
        {
            this.attractiveObjects.Add(attractiveObject.AttractiveObjectId, attractiveObject);
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            List<AttractiveObjectType> attractiveObjectsToDestroy = null;
            foreach (var attractiveObject in attractiveObjects.Values)
            {
                if (attractiveObject.Tick(d, timeAttenuationFactor))
                {
                    if (attractiveObjectsToDestroy == null)
                    {
                        attractiveObjectsToDestroy = new List<AttractiveObjectType>();
                    }
                    attractiveObjectsToDestroy.Add(attractiveObject);
                }
            }
            if (attractiveObjectsToDestroy != null)
            {
                foreach (var attractiveObjectToDestroy in attractiveObjectsToDestroy)
                {
                    this.AttractiveObjectsContainerManager.OnAttractiveObjectDestroy(attractiveObjectToDestroy);
                }
            }
        }

        public void OnAttractiveObjectDestroy(AttractiveObjectType attractiveObjectToDestroy)
        {
            attractiveObjects.Remove(attractiveObjectToDestroy.AttractiveObjectId);
        }

        public AttractiveObjectType GetAttractiveObjectType(AttractiveObjectId attractiveObjectTypeID)
        {
            if (this.attractiveObjects.ContainsKey(attractiveObjectTypeID))
            {
                return this.attractiveObjects[attractiveObjectTypeID];
            }
            return null;
        }
    }
}
