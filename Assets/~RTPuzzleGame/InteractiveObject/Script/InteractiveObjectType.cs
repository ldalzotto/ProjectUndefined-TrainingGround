﻿using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    public class InteractiveObjectType : MonoBehaviour
    {
        [CustomEnum(ConfigurationType = typeof(InteractiveObjectTypeDefinitionConfiguration), OpenToConfiguration = true)]
        public InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID;

        [CustomEnum()]
        public InteractiveObjectID InteractiveObjectID;

        #region Modules
        private Dictionary<Type, InteractiveObjectModule> enabledModules;
        private Dictionary<Type, InteractiveObjectModule> disabledModules;
        #endregion

        #region External Dependencies
        private PuzzleGameConfigurationManager puzzleGameConfigurationManager;
        private InteractiveObjectContainer interactiveObjectContainer;
        private PuzzleEventsManager puzzleEventsManager;
        #endregion

        #region Data Retrieval
        public PuzzleGameConfigurationManager PuzzleGameConfigurationManager { get => puzzleGameConfigurationManager; }
        public PuzzleEventsManager PuzzleEventsManager { get => puzzleEventsManager; }

        public T GetModule<T>() where T : InteractiveObjectModule
        {
            return this.GetModule(typeof(T)) as T;
        }

        public InteractiveObjectModule GetModule(Type moduleType)
        {
            this.enabledModules.TryGetValue(moduleType, out InteractiveObjectModule returnModule);
            if (returnModule != null)
            {
                return returnModule;
            }
            return null;
        }

        public List<InteractiveObjectModule> GetAllModules()
        {
            return this.enabledModules.Values.ToList();
        }

        public T GetDisabledModule<T>() where T : InteractiveObjectModule
        {
            return this.GetDisabledModule(typeof(T)) as T;
        }

        private InteractiveObjectModule GetDisabledModule(Type moduleType)
        {
            this.disabledModules.TryGetValue(moduleType, out InteractiveObjectModule returnModule);
            if (returnModule != null)
            {
                return returnModule;
            }
            return null;
        }

        public List<InteractiveObjectModule> GetAllDisabledModules()
        {
            return this.disabledModules.Values.ToList();
        }

        public T GetEnabledOrDisabledModule<T>() where T : InteractiveObjectModule
        {
            T foundModule = null;
            foundModule = this.GetModule<T>();
            if (foundModule == null) { foundModule = this.GetDisabledModule<T>(); }
            return foundModule;
        }
        #endregion

        public static InteractiveObjectType Instantiate(InteractiveObjectTypeDefinitionInherentData InteractiveObjectTypeDefinitionInherentData,
                        InteractiveObjectInitializationObject InteractiveObjectInitializationObject, PuzzlePrefabConfiguration puzzlePrefabConfiguration, PuzzleGameConfiguration puzzleGameConfiguration,
                        Transform parent = null,
                        List<Type> exclusiveInitialEnabledModules = null)
        {
            InteractiveObjectType InstanciatedInterractiveObject = null;
            if (parent == null)
            {
                InstanciatedInterractiveObject = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseInteractiveObjectType);
            }
            else
            {
                InstanciatedInterractiveObject = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseInteractiveObjectType, parent);
            }
            InteractiveObjectTypeDefinitionInherentData.DefineInteractiveObject(InstanciatedInterractiveObject, puzzlePrefabConfiguration, puzzleGameConfiguration);
            InstanciatedInterractiveObject.Init(InteractiveObjectInitializationObject, exclusiveInitialEnabledModules);
            return InstanciatedInterractiveObject;
        }

        public void Init(InteractiveObjectInitializationObject InteractiveObjectInitializationObject, List<Type> exclusiveInitialEnabledModules = null)
        {
         //   Debug.Log(MyLog.Format(this.InteractiveObjectTypeDefinitionID.ToString()));

            #region External Dependencies
            this.puzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            this.interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            this.puzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            var puzzleGameStatciConfigurationContainer = GameObject.FindObjectOfType<PuzzleStaticConfigurationContainer>();
            #endregion

            if (InteractiveObjectTypeDefinitionID != InteractiveObjectTypeDefinitionID.NONE)
            {
                this.puzzleGameConfigurationManager.InteractiveObjectTypeDefinitionConfiguration()[this.InteractiveObjectTypeDefinitionID].DefineInteractiveObject(this,
                         puzzleGameStatciConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration, puzzleGameConfigurationManager.PuzzleGameConfiguration);
            }

            #region Internal Dependencies
            InteractiveObjectInitializationObject.TransformMoveManagerComponent = GetComponent<InteractiveObjectSharedDataType>();
            #endregion

            this.PopulateModules(exclusiveInitialEnabledModules);

            foreach (var initializationStatement in InteractiveObjectTypeConfiguration.InitializationConfiguration.Values)
            {
                initializationStatement.Invoke(InteractiveObjectInitializationObject, this);
            }

            this.interactiveObjectContainer.OnInteractiveObjectAdded(this);
        }

        private void PopulateModules(List<Type> exclusiveInitialEnabledModules)
        {
            this.enabledModules = new Dictionary<Type, InteractiveObjectModule>();
            this.disabledModules = new Dictionary<Type, InteractiveObjectModule>();
            var retrievedInteractiveObjectModules = this.GetComponentsInChildren<InteractiveObjectModule>();
            if (retrievedInteractiveObjectModules != null)
            {
                foreach (var interactiveObjectModule in retrievedInteractiveObjectModules)
                {
                    this.EnableModuleOnInit(interactiveObjectModule);
                }
            }

            if (exclusiveInitialEnabledModules != null)
            {
                foreach (var enabledModulesEntry in this.enabledModules.Keys.ToList())
                {
                    if (!exclusiveInitialEnabledModules.Contains(enabledModulesEntry))
                    {
                        this.DisableModule(enabledModulesEntry);
                    }
                }
            }
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.GetModule<DisarmObjectModule>().IfNotNull((DisarmObjectModule disarmObjectModule) => disarmObjectModule.Tick(d, timeAttenuationFactor));
            this.GetModule<AttractiveObjectModule>().IfNotNull((AttractiveObjectModule attractiveObjectTypeModule) => attractiveObjectTypeModule.Tick(d, timeAttenuationFactor));
            this.GetModule<ObjectRepelModule>().IfNotNull((ObjectRepelModule objectRepelTypeModule) => objectRepelTypeModule.Tick(d, timeAttenuationFactor));
            this.GetModule<LaunchProjectileModule>().IfNotNull((LaunchProjectileModule launchProjectileModule) => launchProjectileModule.Tick(d, timeAttenuationFactor));
            this.GetModule<ActionInteractableObjectModule>().IfNotNull((ActionInteractableObjectModule actionInteractableObjectModule) => actionInteractableObjectModule.Tick(d, timeAttenuationFactor));
            this.GetModule<InteractiveObjectCutsceneControllerModule>().IfNotNull((InteractiveObjectCutsceneControllerModule interactiveObjectCutsceneControllerModule) => interactiveObjectCutsceneControllerModule.Tick(d, timeAttenuationFactor));
        }

        public void TickBeforeAIUpdate(float d, float timeAttenuationFactor)
        {
            this.GetModule<ObjectSightModule>().IfNotNull((ObjectSightModule ObjectSightModule) => ObjectSightModule.TickBeforeAIUpdate(d));
        }

        public void TickAlways(float d)
        {
            this.GetModule<DisarmObjectModule>().IfNotNull((DisarmObjectModule disarmObjectModule) => disarmObjectModule.TickAlways(d));
        }

        public void DisableModule(Type moduleType)
        {
            this.GetModule(moduleType).IfNotNull((m) =>
            {
                m.gameObject.SetActive(false);
                this.enabledModules.Remove(moduleType);
                this.disabledModules[moduleType] = m;
                this.interactiveObjectContainer.OnModuleDisabled(m);
            });
        }

        private void EnableModuleOnInit(InteractiveObjectModule moduleToEnable)
        {
            this.enabledModules[moduleToEnable.GetType()] = moduleToEnable;
            this.interactiveObjectContainer.OnModuleEnabled(moduleToEnable);
        }

        public void EnableModule(Type moduleType, InteractiveObjectInitializationObject InteractiveObjectInitializationObject)
        {
            this.GetDisabledModule(moduleType).IfNotNull((m) =>
            {
                m.gameObject.SetActive(true);
                this.disabledModules.Remove(moduleType);
                this.enabledModules[moduleType] = m;
                this.interactiveObjectContainer.OnModuleEnabled(m);
                InteractiveObjectTypeConfiguration.InitializationConfiguration[moduleType].Invoke(InteractiveObjectInitializationObject, this);
            });

        }

        internal void EnableAllDisabledModules(InteractiveObjectInitializationObject InteractiveObjectInitializationObject)
        {
            foreach (var disabledModule in this.disabledModules.Keys.ToList())
            {
                this.EnableModule(disabledModule, InteractiveObjectInitializationObject);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.Label(this.transform.position + new Vector3(0, -2f, 0), this.InteractiveObjectTypeDefinitionID.ToString(), MyEditorStyles.LabelWhite);
        }
#endif
    }


}
