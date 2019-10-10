using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class InteractiveObjectContainer : MonoBehaviour
    {
        #region Generic Container
        private MultiValueDictionary<InteractiveObjectID, InteractiveObjectType> interactiveObjects;
        #endregion

        #region Specific Containers
        private MultiValueDictionary<AttractiveObjectId, AttractiveObjectModule> attractiveObjectContainer;
        private List<IObjectRepelModuleDataRetrieval> objectsRepelable;
        private Dictionary<TargetZoneID, ITargetZoneModuleDataRetriever> targetZones;
        private List<IVisualFeedbackEmitterModuleDataRetriever> visualFeedbackEmitterModules;
        #endregion

        #region Data Retrieval
        public List<IObjectRepelModuleDataRetrieval> ObjectsRepelable { get => objectsRepelable; }
        public Dictionary<TargetZoneID, ITargetZoneModuleDataRetriever> TargetZones { get => targetZones; }
        public List<IVisualFeedbackEmitterModuleDataRetriever> VisualFeedbackEmitterModules { get => visualFeedbackEmitterModules; }

        public InteractiveObjectType GetInteractiveObjectFirst(InteractiveObjectID interactiveObjectID)
        {
            return this.GetIntractiveObjectsAll(interactiveObjectID)[0];
        }
        public List<InteractiveObjectType> GetIntractiveObjectsAll(InteractiveObjectID interactiveObjectID)
        {
            return this.interactiveObjects[interactiveObjectID];
        }
        #endregion

        public void Init()
        {
            this.interactiveObjects = new MultiValueDictionary<InteractiveObjectID, InteractiveObjectType>();
            this.attractiveObjectContainer = new MultiValueDictionary<AttractiveObjectId, AttractiveObjectModule>();
            this.objectsRepelable = new List<IObjectRepelModuleDataRetrieval>();
            this.targetZones = new Dictionary<TargetZoneID, ITargetZoneModuleDataRetriever>();
            this.visualFeedbackEmitterModules = new List<IVisualFeedbackEmitterModuleDataRetriever>();

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
            foreach (var interactiveObject in interactiveObjects.MultiValueGetValues())
            {
                interactiveObject.Tick(d, timeAttenuationFactor);
            }

            ManagerDestroyAfterTick();
        }

        public void TickBeforeAIUpdate(float d, float timeAttenuationFactor)
        {
            foreach (var interactiveObject in interactiveObjects.MultiValueGetValues())
            {
                interactiveObject.TickBeforeAIUpdate(d, timeAttenuationFactor);
            }

            ManagerDestroyAfterTick();
        }

        public void TickAlways(float d)
        {
            foreach (var interactiveObject in interactiveObjects.MultiValueGetValues())
            {
                interactiveObject.TickAlways(d);
            }

            this.ManagerDestroyAfterTick();
        }

        private void ManagerDestroyAfterTick()
        {
           
        }

        #region External Event
        public void OnInteractiveObjectAdded(InteractiveObjectType interactiveObject)
        {
            this.interactiveObjects.MultiValueAdd(interactiveObject.InteractiveObjectID, interactiveObject);
        }

        public void OnModuleEnabled(InteractiveObjectModule enabledModule)
        {
            if (enabledModule.GetType() == typeof(AttractiveObjectModule))
            {
                var AttractiveObjectTypeModule = ((AttractiveObjectModule)enabledModule);
                this.attractiveObjectContainer.MultiValueAdd(AttractiveObjectTypeModule.AttractiveObjectId, AttractiveObjectTypeModule);
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
            else if (enabledModule.GetType() == typeof(VisualFeedbackEmitterModule))
            {
                this.visualFeedbackEmitterModules.Add((VisualFeedbackEmitterModule)enabledModule);
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
            else if (enabledModule.GetType() == typeof(VisualFeedbackEmitterModule))
            {
                this.visualFeedbackEmitterModules.Remove((VisualFeedbackEmitterModule)enabledModule);
            }
        }

        public void OnInteractiveObjectDestroyed(InteractiveObjectType interactiveObject)
        {
            OnInteractiveObjectDestroyedLogic(interactiveObject);
            MonoBehaviour.Destroy(interactiveObject.gameObject);
        }

        private void OnInteractiveObjectDestroyedLogic(InteractiveObjectType interactiveObject)
        {
            this.interactiveObjects.MultiValueRemove(interactiveObject.InteractiveObjectID, interactiveObject);

            #region Call events for all modules
            interactiveObject.OnInteractiveObjectDestroyed();
            #endregion

            interactiveObject.GetModule<AttractiveObjectModule>().IfNotNull((AttractiveObjectModule AttractiveObjectTypeModule) => this.attractiveObjectContainer.MultiValueRemove(AttractiveObjectTypeModule.AttractiveObjectId, AttractiveObjectTypeModule));
            interactiveObject.GetModule<ObjectRepelModule>().IfNotNull((ObjectRepelModule ObjectRepelTypeModule) => this.objectsRepelable.Remove(ObjectRepelTypeModule));
            interactiveObject.GetModule<TargetZoneModule>().IfNotNull((TargetZoneModule TargetZoneObjectModule) => this.targetZones.Remove(TargetZoneObjectModule.TargetZoneID));
            interactiveObject.GetModule<VisualFeedbackEmitterModule>().IfNotNull((VisualFeedbackEmitterModule InRangeVisualFeedbackModule) => this.visualFeedbackEmitterModules.Remove(InRangeVisualFeedbackModule));
        }

        public void OnGameOver()
        {
            foreach (var interactiveObject in this.interactiveObjects.MultiValueGetValues())
            {
                interactiveObject.GetIContextMarkVisualFeedbackEvent().IfNotNull((IContextMarkVisualFeedbackEvent) => IContextMarkVisualFeedbackEvent.Delete());
            }
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
