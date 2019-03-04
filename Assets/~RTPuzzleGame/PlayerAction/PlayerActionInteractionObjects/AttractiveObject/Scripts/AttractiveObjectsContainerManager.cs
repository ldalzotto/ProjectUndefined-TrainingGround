using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class AttractiveObjectsContainerManager : MonoBehaviour
    {
        private AttractiveObjectsContainer AttractiveObjectsContainer;

        public void Init()
        {
            AttractiveObjectsContainer = new AttractiveObjectsContainer();
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            AttractiveObjectsContainer.Tick(d);
        }

        #region External Events
        public void OnAttractiveObjectActionExecuted(Vector3 attractiveObjectWorldPosition, AttractiveObjectInherentConfigurationData attractiveObjectInherentConfigurationData)
        {
            var instanciatedAttractiveObject = AttractiveObjectType.Instanciate(attractiveObjectWorldPosition, transform, attractiveObjectInherentConfigurationData.EffectRange);
            AttractiveObjectsContainer.AddAttractiveObject(instanciatedAttractiveObject);
        }
        #endregion
    }

    class AttractiveObjectsContainer
    {
        private List<AttractiveObjectType> attractiveObjects;

        public AttractiveObjectsContainer()
        {
            this.attractiveObjects = new List<AttractiveObjectType>();
        }

        public void AddAttractiveObject(AttractiveObjectType attractiveObject)
        {
            this.attractiveObjects.Add(attractiveObject);
        }

        public void Tick(float d)
        {
            foreach (var attractiveObject in attractiveObjects)
            {

            }
        }
    }
}
