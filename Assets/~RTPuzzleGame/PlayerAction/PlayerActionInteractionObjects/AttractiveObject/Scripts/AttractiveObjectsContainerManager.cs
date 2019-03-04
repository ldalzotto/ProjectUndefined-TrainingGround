using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class AttractiveObjectsContainerManager : MonoBehaviour
    {
        #region External Dependencies
        private NPCAIManagerContainer NPCAIManagerContainer;
        #endregion

        private AttractiveObjectsContainer AttractiveObjectsContainer;

        public void Init()
        {
            this.NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            AttractiveObjectsContainer = new AttractiveObjectsContainer(this);
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            AttractiveObjectsContainer.Tick(d, timeAttenuationFactor);
        }

        #region External Events
        public void OnAttractiveObjectActionExecuted(Vector3 attractiveObjectWorldPosition, AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData)
        {
            var instanciatedAttractiveObject = AttractiveObjectType.Instanciate(attractiveObjectWorldPosition, transform, attractiveObjectInherentConfigurationData);
            AttractiveObjectsContainer.AddAttractiveObject(instanciatedAttractiveObject);
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
    }

    class AttractiveObjectsContainer
    {
        private List<AttractiveObjectType> attractiveObjects;
        private AttractiveObjectsContainerManager AttractiveObjectsContainerManager;

        public AttractiveObjectsContainer(AttractiveObjectsContainerManager AttractiveObjectsContainerManager)
        {
            this.AttractiveObjectsContainerManager = AttractiveObjectsContainerManager;
            this.attractiveObjects = new List<AttractiveObjectType>();
        }

        public void AddAttractiveObject(AttractiveObjectType attractiveObject)
        {
            this.attractiveObjects.Add(attractiveObject);
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            List<AttractiveObjectType> attractiveObjectsToDestroy = null;
            foreach (var attractiveObject in attractiveObjects)
            {
               if(attractiveObject.Tick(d, timeAttenuationFactor))
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
                foreach(var attractiveObjectToDestroy in attractiveObjectsToDestroy)
                {
                    this.AttractiveObjectsContainerManager.OnAttractiveObjectDestroy(attractiveObjectToDestroy);
                }
            }
        }

        public void OnAttractiveObjectDestroy(AttractiveObjectType attractiveObjectToDestroy)
        {
            attractiveObjects.Remove(attractiveObjectToDestroy);
        }
    }
}
