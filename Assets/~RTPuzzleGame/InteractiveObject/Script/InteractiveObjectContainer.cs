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
        private DisarmObjectProgressBarRendererManager DisarmObjectProgressBarRendererManager;
        #endregion

        #region Generic Container
        private Dictionary<InteractiveObjectID, List<InteractiveObjectType>> interactiveObjects;
        #endregion

        #region Specific Containers
        private Dictionary<AttractiveObjectId, AttractiveObjectModule> attractiveObjectContainer;
        private List<ObjectRepelModule> objectsRepelable;
        private Dictionary<TargetZoneID, TargetZoneModule> targetZones;
        private List<DisarmObjectModule> disarmObjectModules;
        private Dictionary<GrabObjectID, GrabObjectModule> grabObjectModules;
        #endregion

        #region Data Retrieval
        public AttractiveObjectModule GetAttractiveObjectType(AttractiveObjectId attractiveObjectId)
        {
            return this.attractiveObjectContainer[attractiveObjectId];
        }
        public List<ObjectRepelModule> ObjectsRepelable { get => objectsRepelable; }
        public Dictionary<TargetZoneID, TargetZoneModule> TargetZones { get => targetZones; }
        public List<DisarmObjectModule> DisarmObjectModules { get => disarmObjectModules; }
        public Dictionary<GrabObjectID, GrabObjectModule> GrabObjectModules { get => grabObjectModules; }
        #endregion

        public void Init()
        {
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            this.DisarmObjectProgressBarRendererManager = GameObject.FindObjectOfType<DisarmObjectProgressBarRendererManager>();

            this.interactiveObjects = new Dictionary<InteractiveObjectID, List<InteractiveObjectType>>();
            this.attractiveObjectContainer = new Dictionary<AttractiveObjectId, AttractiveObjectModule>();
            this.objectsRepelable = new List<ObjectRepelModule>();
            this.targetZones = new Dictionary<TargetZoneID, TargetZoneModule>();
            this.disarmObjectModules = new List<DisarmObjectModule>();
            this.grabObjectModules = new Dictionary<GrabObjectID, GrabObjectModule>();

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
            foreach (var idAssociatedInteractiveObjects in interactiveObjects.Values)
            {
                foreach (var interactiveObject in idAssociatedInteractiveObjects)
                {
                    interactiveObject.Tick(d, timeAttenuationFactor);
                }
            }

            //After Tick
            List<InteractiveObjectType> interactiveObjectsToRemove = null;

            foreach (var idAssociatedInteractiveObjects in interactiveObjects.Values)
            {
                foreach (var interactiveObject in idAssociatedInteractiveObjects)
                {
                    if ((interactiveObject.GetModule<AttractiveObjectModule>() != null && interactiveObject.GetModule<AttractiveObjectModule>().IsAskingToBeDestroyed())
                    || (interactiveObject.GetModule<DisarmObjectModule>() != null && interactiveObject.GetModule<DisarmObjectModule>().IsAskingToBeDestroyed())
                   )
                    {
                        if (interactiveObjectsToRemove == null) { interactiveObjectsToRemove = new List<InteractiveObjectType>(); }
                        interactiveObjectsToRemove.Add(interactiveObject);
                    }
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
            if (!this.interactiveObjects.ContainsKey(interactiveObject.InteractiveObjectID))
            {
                this.interactiveObjects.Add(interactiveObject.InteractiveObjectID, new List<InteractiveObjectType>());
            }
            this.interactiveObjects[interactiveObject.InteractiveObjectID].Add(interactiveObject);
        }

        public void OnModuleEnabled(InteractiveObjectModule enabledModule)
        {
            if (enabledModule.GetType() == typeof(AttractiveObjectModule))
            {
                var AttractiveObjectTypeModule = ((AttractiveObjectModule)enabledModule);
                this.attractiveObjectContainer.Add(AttractiveObjectTypeModule.AttractiveObjectId, AttractiveObjectTypeModule);
            }
            else if (enabledModule.GetType() == typeof(ObjectRepelModule))
            {
                var ObjectRepelTypeModule = ((ObjectRepelModule)enabledModule);
                this.objectsRepelable.Add(ObjectRepelTypeModule);
            }
            else if (enabledModule.GetType() == typeof(TargetZoneModule))
            {
                var TargetZoneObjectModule = ((TargetZoneModule)enabledModule);
                this.targetZones.Add(TargetZoneObjectModule.TargetZoneID, TargetZoneObjectModule);
            }
            else if (enabledModule.GetType() == typeof(DisarmObjectModule))
            {
                var DisarmObjectModule = ((DisarmObjectModule)enabledModule);
                this.disarmObjectModules.Add(DisarmObjectModule);
            }
            else if (enabledModule.GetType() == typeof(GrabObjectModule))
            {
                var GrabObjectModule = ((GrabObjectModule)enabledModule);
                this.grabObjectModules.Add(GrabObjectModule.GrabObjectID, GrabObjectModule);
            }
        }

        public void OnModuleDisabled(InteractiveObjectModule enabledModule)
        {
            if (enabledModule.GetType() == typeof(AttractiveObjectModule))
            {
                var AttractiveObjectTypeModule = ((AttractiveObjectModule)enabledModule);
                this.attractiveObjectContainer.Remove(AttractiveObjectTypeModule.AttractiveObjectId);
            }
            else if (enabledModule.GetType() == typeof(ObjectRepelModule))
            {
                var ObjectRepelTypeModule = ((ObjectRepelModule)enabledModule);
                this.objectsRepelable.Remove(ObjectRepelTypeModule);
            }
            else if (enabledModule.GetType() == typeof(TargetZoneModule))
            {
                var TargetZoneObjectModule = ((TargetZoneModule)enabledModule);
                this.targetZones.Remove(TargetZoneObjectModule.TargetZoneID);
            }
            else if (enabledModule.GetType() == typeof(DisarmObjectModule))
            {
                var DisarmObjectModule = ((DisarmObjectModule)enabledModule);
                this.disarmObjectModules.Remove(DisarmObjectModule);
            }
            else if (enabledModule.GetType() == typeof(GrabObjectModule))
            {
                var GrabObjectModule = ((GrabObjectModule)enabledModule);
                this.grabObjectModules.Remove(GrabObjectModule.GrabObjectID);
            }
        }

        public void OnInteractiveObjectDestroyed(InteractiveObjectType interactiveObject)
        {
            OnInteractiveObjectDestroyedLogic(interactiveObject);
            MonoBehaviour.Destroy(interactiveObject.gameObject);
        }

        private void OnInteractiveObjectDestroyedLogic(InteractiveObjectType interactiveObject)
        {
            this.interactiveObjects[interactiveObject.InteractiveObjectID].Remove(interactiveObject);
            if (this.interactiveObjects[interactiveObject.InteractiveObjectID].Count == 0)
            {
                this.interactiveObjects.Remove(interactiveObject.InteractiveObjectID);
            }

            #region AttractiveObjectTypeModule
            interactiveObject.GetModule<AttractiveObjectModule>().IfNotNull((AttractiveObjectModule AttractiveObjectTypeModule) =>
            {
                this.attractiveObjectContainer.Remove(AttractiveObjectTypeModule.AttractiveObjectId);
                AttractiveObjectTypeModule.SphereRange.OnRangeDestroyed();
                this.PuzzleEventsManager.PZ_EVT_AttractiveObject_TpeDestroyed(AttractiveObjectTypeModule);
            });
            #endregion

            interactiveObject.GetModule<ObjectRepelModule>().IfNotNull((ObjectRepelModule ObjectRepelTypeModule) => this.objectsRepelable.Remove(ObjectRepelTypeModule));
            interactiveObject.GetModule<TargetZoneModule>().IfNotNull((TargetZoneModule TargetZoneObjectModule) => this.targetZones.Remove(TargetZoneObjectModule.TargetZoneID));
            interactiveObject.GetModule<DisarmObjectModule>().IfNotNull((DisarmObjectModule DisarmObjectModule) =>
            {
                this.DisarmObjectProgressBarRendererManager.OnDisarmObjectDestroyed(DisarmObjectModule);
                this.disarmObjectModules.Remove(DisarmObjectModule);
            });
            interactiveObject.GetModule<GrabObjectModule>().IfNotNull((GrabObjectModule GrabObjectModule) =>
            {
                GrabObjectModule.OnInteractiveObjectDestroyed();
                this.grabObjectModules.Remove(GrabObjectModule.GrabObjectID);
            });

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
