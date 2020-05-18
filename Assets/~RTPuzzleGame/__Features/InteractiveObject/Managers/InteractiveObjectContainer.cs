﻿using CoreGame;
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
        private List<DisarmObjectModule> disarmObjectModules;
        private Dictionary<GrabObjectID, GrabObjectModule> grabObjectModules;
        private List<IInRangeVisualFeedbackModuleDataRetriever> inRangeVisualFeedbackModules;
        #endregion

        #region Data Retrieval
        public List<IObjectRepelModuleDataRetrieval> ObjectsRepelable { get => objectsRepelable; }
        public Dictionary<TargetZoneID, ITargetZoneModuleDataRetriever> TargetZones { get => targetZones; }
        public List<DisarmObjectModule> DisarmObjectModules { get => disarmObjectModules; }
        public Dictionary<GrabObjectID, GrabObjectModule> GrabObjectModules { get => grabObjectModules; }
        public List<IInRangeVisualFeedbackModuleDataRetriever> InRangeVisualFeedbackModules { get => inRangeVisualFeedbackModules; }

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
            this.disarmObjectModules = new List<DisarmObjectModule>();
            this.grabObjectModules = new Dictionary<GrabObjectID, GrabObjectModule>();
            this.inRangeVisualFeedbackModules = new List<IInRangeVisualFeedbackModuleDataRetriever>();

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
            //After Tick
            List<InteractiveObjectType> interactiveObjectsToRemove = null;

            foreach (var interactiveObject in interactiveObjects.MultiValueGetValues())
            {
                if ((interactiveObject.GetModule<AttractiveObjectModule>() != null && interactiveObject.GetModule<AttractiveObjectModule>().IsAskingToBeDestroyed())
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
            else if (enabledModule.GetType() == typeof(InRangeVisualFeedbackModule))
            {
                this.inRangeVisualFeedbackModules.Add((InRangeVisualFeedbackModule)enabledModule);
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
            else if (enabledModule.GetType() == typeof(InRangeVisualFeedbackModule))
            {
                this.inRangeVisualFeedbackModules.Remove((InRangeVisualFeedbackModule)enabledModule);
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
            interactiveObject.GetModule<DisarmObjectModule>().IfNotNull((DisarmObjectModule DisarmObjectModule) => this.disarmObjectModules.Remove(DisarmObjectModule));
            interactiveObject.GetModule<GrabObjectModule>().IfNotNull((GrabObjectModule GrabObjectModule) => this.grabObjectModules.Remove(GrabObjectModule.GrabObjectID));
            interactiveObject.GetModule<InRangeVisualFeedbackModule>().IfNotNull((InRangeVisualFeedbackModule InRangeVisualFeedbackModule) => this.inRangeVisualFeedbackModules.Remove(InRangeVisualFeedbackModule));
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
