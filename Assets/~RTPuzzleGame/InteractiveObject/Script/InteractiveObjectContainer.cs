using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class InteractiveObjectContainer : MonoBehaviour
    {
        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        #region Generic Container
        private List<InteractiveObjectType> interactiveObjects;
        #endregion

        #region Specific Containers
        private Dictionary<AttractiveObjectId, AttractiveObjectTypeModule> attractiveObjectContainer;
        private List<ObjectRepelTypeModule> objectsRepelable;
        #endregion

        #region Data Retrieval
        public AttractiveObjectTypeModule GetAttractiveObjectType(AttractiveObjectId attractiveObjectId)
        {
            return this.attractiveObjectContainer[attractiveObjectId];
        }
        public List<ObjectRepelTypeModule> ObjectsRepelable { get => objectsRepelable;  }
        #endregion

        public void Init()
        {
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();

            this.interactiveObjects = new List<InteractiveObjectType>();
            this.attractiveObjectContainer = new Dictionary<AttractiveObjectId, AttractiveObjectTypeModule>();
            this.objectsRepelable = new List<ObjectRepelTypeModule>();

            var allStartInteractiveObjects = GameObject.FindObjectsOfType<InteractiveObjectType>();
            if (allStartInteractiveObjects != null)
            {
                foreach (var interactiveObject in allStartInteractiveObjects)
                {
                    interactiveObject.Init();
                }
            }
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            foreach (var interactiveObject in interactiveObjects)
            {
                interactiveObject.Tick(d, timeAttenuationFactor);
            }

            //After Tick
            List<InteractiveObjectType> interactiveObjectsToRemove = null;
            foreach (var interactiveObject in interactiveObjects)
            {
                if (interactiveObject.AttractiveObjectTypeModule != null && interactiveObject.AttractiveObjectTypeModule.IsAskingToBeDestroyed())
                {
                    if (interactiveObjectsToRemove == null) { interactiveObjectsToRemove = new List<InteractiveObjectType>(); }
                    interactiveObjectsToRemove.Add(interactiveObject);
                }
            }

            if (interactiveObjectsToRemove != null)
            {
                foreach (var interactiveObjectToRemove in interactiveObjectsToRemove)
                {
                    this.OnInteractiveObjectDestroyed(interactiveObjectToRemove);
                }
            }
        }

        #region External Event
        public void OnInteractiveObjectAdded(InteractiveObjectType interactiveObject)
        {
            this.interactiveObjects.Add(interactiveObject);
            interactiveObject.AttractiveObjectTypeModule.IfNotNull((AttractiveObjectTypeModule AttractiveObjectTypeModule) => this.attractiveObjectContainer.Add(AttractiveObjectTypeModule.AttractiveObjectId, AttractiveObjectTypeModule));
            interactiveObject.ObjectRepelTypeModule.IfNotNull((ObjectRepelTypeModule ObjectRepelTypeModule) => this.objectsRepelable.Add(ObjectRepelTypeModule));
        }

        public void OnInteractiveObjectDestroyed(InteractiveObjectType interactiveObject)
        {
            this.interactiveObjects.Remove(interactiveObject);

            #region AttractiveObjectTypeModule
            interactiveObject.AttractiveObjectTypeModule.IfNotNull((AttractiveObjectTypeModule AttractiveObjectTypeModule) =>
            {
                this.attractiveObjectContainer.Remove(AttractiveObjectTypeModule.AttractiveObjectId);
                AttractiveObjectTypeModule.SphereRange.OnRangeDestroyed();
                this.PuzzleEventsManager.PZ_EVT_AttractiveObject_TpeDestroyed(AttractiveObjectTypeModule);
            });
            #endregion

            interactiveObject.ObjectRepelTypeModule.IfNotNull((ObjectRepelTypeModule ObjectRepelTypeModule) => this.objectsRepelable.Remove(ObjectRepelTypeModule));

            MonoBehaviour.Destroy(interactiveObject.gameObject);
        }
        #endregion

    }
}
