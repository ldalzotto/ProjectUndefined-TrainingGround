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
        private Dictionary<TargetZoneID, TargetZoneObjectModule> targetZones;
        private List<DisarmObjectModule> disarmObjectModules;
        #endregion

        #region Data Retrieval
        public AttractiveObjectTypeModule GetAttractiveObjectType(AttractiveObjectId attractiveObjectId)
        {
            return this.attractiveObjectContainer[attractiveObjectId];
        }
        public List<ObjectRepelTypeModule> ObjectsRepelable { get => objectsRepelable; }
        public Dictionary<TargetZoneID, TargetZoneObjectModule> TargetZones { get => targetZones; }
        #endregion

        public void Init()
        {
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();

            this.interactiveObjects = new List<InteractiveObjectType>();
            this.attractiveObjectContainer = new Dictionary<AttractiveObjectId, AttractiveObjectTypeModule>();
            this.objectsRepelable = new List<ObjectRepelTypeModule>();
            this.targetZones = new Dictionary<TargetZoneID, TargetZoneObjectModule>();
            this.disarmObjectModules = new List<DisarmObjectModule>();

            var allStartInteractiveObjects = GameObject.FindObjectsOfType<InteractiveObjectType>();
            if (allStartInteractiveObjects != null)
            {
                foreach (var interactiveObject in allStartInteractiveObjects)
                {
                    interactiveObject.Init(new InteractiveObjectInitializationObject());
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
                if ((interactiveObject.GetModule<AttractiveObjectTypeModule>() != null && interactiveObject.GetModule<AttractiveObjectTypeModule>().IsAskingToBeDestroyed())
                    || (interactiveObject.GetModule<DisarmObjectModule>() != null && interactiveObject.GetModule<DisarmObjectModule>().IsAskingToBeDestroyed())
                   )
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
        }

        public void OnModuleEnabled(InteractiveObjectModule enabledModule)
        {
            if (enabledModule.GetType() == typeof(AttractiveObjectTypeModule))
            {
                var AttractiveObjectTypeModule = ((AttractiveObjectTypeModule)enabledModule);
                this.attractiveObjectContainer.Add(AttractiveObjectTypeModule.AttractiveObjectId, AttractiveObjectTypeModule);
            }
            else if (enabledModule.GetType() == typeof(ObjectRepelTypeModule))
            {
                var ObjectRepelTypeModule = ((ObjectRepelTypeModule)enabledModule);
                this.objectsRepelable.Add(ObjectRepelTypeModule);
            }
            else if (enabledModule.GetType() == typeof(TargetZoneObjectModule))
            {
                var TargetZoneObjectModule = ((TargetZoneObjectModule)enabledModule);
                this.targetZones.Add(TargetZoneObjectModule.TargetZoneID, TargetZoneObjectModule);
            }
            else if (enabledModule.GetType() == typeof(DisarmObjectModule))
            {
                var DisarmObjectModule = ((DisarmObjectModule)enabledModule);
                this.disarmObjectModules.Add(DisarmObjectModule);
            }
        }

        public void OnModuleDisabled(InteractiveObjectModule enabledModule)
        {
            if (enabledModule.GetType() == typeof(AttractiveObjectTypeModule))
            {
                var AttractiveObjectTypeModule = ((AttractiveObjectTypeModule)enabledModule);
                this.attractiveObjectContainer.Remove(AttractiveObjectTypeModule.AttractiveObjectId);
            }
            else if (enabledModule.GetType() == typeof(ObjectRepelTypeModule))
            {
                var ObjectRepelTypeModule = ((ObjectRepelTypeModule)enabledModule);
                this.objectsRepelable.Remove(ObjectRepelTypeModule);
            }
            else if (enabledModule.GetType() == typeof(TargetZoneObjectModule))
            {
                var TargetZoneObjectModule = ((TargetZoneObjectModule)enabledModule);
                this.targetZones.Remove(TargetZoneObjectModule.TargetZoneID);
            }
            else if (enabledModule.GetType() == typeof(DisarmObjectModule))
            {
                var DisarmObjectModule = ((DisarmObjectModule)enabledModule);
                this.disarmObjectModules.Remove(DisarmObjectModule);
            }
        }

        public void OnInteractiveObjectDestroyed(InteractiveObjectType interactiveObject)
        {
            OnInteractiveObjectDestroyedLogic(interactiveObject);
            MonoBehaviour.Destroy(interactiveObject.gameObject);
        }

        private void OnInteractiveObjectDestroyedLogic(InteractiveObjectType interactiveObject)
        {
            this.interactiveObjects.Remove(interactiveObject);

            #region AttractiveObjectTypeModule
            interactiveObject.GetModule<AttractiveObjectTypeModule>().IfNotNull((AttractiveObjectTypeModule AttractiveObjectTypeModule) =>
            {
                this.attractiveObjectContainer.Remove(AttractiveObjectTypeModule.AttractiveObjectId);
                AttractiveObjectTypeModule.SphereRange.OnRangeDestroyed();
                this.PuzzleEventsManager.PZ_EVT_AttractiveObject_TpeDestroyed(AttractiveObjectTypeModule);
            });
            #endregion

            interactiveObject.GetModule<ObjectRepelTypeModule>().IfNotNull((ObjectRepelTypeModule ObjectRepelTypeModule) => this.objectsRepelable.Remove(ObjectRepelTypeModule));
            interactiveObject.GetModule<TargetZoneObjectModule>().IfNotNull((TargetZoneObjectModule TargetZoneObjectModule) => this.targetZones.Remove(TargetZoneObjectModule.TargetZoneID));
            interactiveObject.GetModule<DisarmObjectModule>().IfNotNull((DisarmObjectModule DisarmObjectModule) => this.disarmObjectModules.Remove(DisarmObjectModule));

            #region LevelCompletionTriggerModule
            interactiveObject.GetModule<LevelCompletionTriggerModule>().IfNotNull((LevelCompletionTriggerModule LevelCompletionTriggerModule) =>
            {
                LevelCompletionTriggerModule.OnInteractiveObjectDestroyed();
            });
            #endregion
        }


#if UNITY_EDITOR
        public void TEST_OnInteractiveObjectDestroyed(InteractiveObjectType interactiveObject)
        {
            this.OnInteractiveObjectDestroyedLogic(interactiveObject);
            MonoBehaviour.DestroyImmediate(interactiveObject.gameObject);
        }
#endif
        #endregion

    }
}
