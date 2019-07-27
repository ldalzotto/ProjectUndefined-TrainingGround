using CoreGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class InteractiveObjectType : MonoBehaviour
    {

        #region Modules
        private Dictionary<Type, InteractiveObjectModule> enabledModules;
        private Dictionary<Type, InteractiveObjectModule> disabledModules;
        #endregion
        
        #region External Dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private InteractiveObjectContainer InteractiveObjectContainer;
        #endregion


        #region Data Retrieval
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
        #endregion

        public void Init(InteractiveObjectInitializationObject InteractiveObjectInitializationObject, List<Type> exclusiveInitialEnabledModules = null)
        {
            #region External Dependencies
            this.PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            this.InteractiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            #endregion

            this.PopulateModules(exclusiveInitialEnabledModules);

            this.InitializeModelObjectModule();
            this.InitializeAttractiveObjectTypeModule(InteractiveObjectInitializationObject);
            this.IntitializeObjectRepelTypeModule();
            this.InitializeLevelCompletionTriggerModule();
            this.InitializeTargetZoneObjectModule(InteractiveObjectInitializationObject);
            this.InitializeProjectileModule(InteractiveObjectInitializationObject);


            this.InteractiveObjectContainer.OnInteractiveObjectAdded(this);
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
                    interactiveObjectModule.IfTypeEqual((AttractiveObjectTypeModule interactiveObjectModule2) => this.EnableModuleOnInit(interactiveObjectModule2));
                    interactiveObjectModule.IfTypeEqual((ModelObjectModule interactiveObjectModule2) => this.EnableModuleOnInit(interactiveObjectModule2));
                    interactiveObjectModule.IfTypeEqual((ObjectRepelTypeModule interactiveObjectModule2) => this.EnableModuleOnInit(interactiveObjectModule2));
                    interactiveObjectModule.IfTypeEqual((LevelCompletionTriggerModule interactiveObjectModule2) => this.EnableModuleOnInit(interactiveObjectModule2));
                    interactiveObjectModule.IfTypeEqual((TargetZoneObjectModule interactiveObjectModule2) => this.EnableModuleOnInit(interactiveObjectModule2));
                    interactiveObjectModule.IfTypeEqual((LaunchProjectileModule launchProjectileModule2) => this.EnableModuleOnInit(launchProjectileModule2));
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

        #region Initialization

        private void InitializeModelObjectModule()
        {
            this.GetModule<ModelObjectModule>().IfNotNull((ModelObjectModule modelObjectModule) => modelObjectModule.Init());
        }

        private void IntitializeObjectRepelTypeModule()
        {
            this.GetModule<ObjectRepelTypeModule>().IfNotNull((ObjectRepelTypeModule objectRepelTypeModule) => objectRepelTypeModule.Init(this.GetModule<ModelObjectModule>()));
        }

        private void InitializeLevelCompletionTriggerModule()
        {
            this.GetModule<LevelCompletionTriggerModule>().IfNotNull((LevelCompletionTriggerModule levelCompletionTriggerModule) => levelCompletionTriggerModule.Init());
        }

        private void InitializeTargetZoneObjectModule(InteractiveObjectInitializationObject InteractiveObjectInitializationObject)
        {
            this.GetModule<TargetZoneObjectModule>().IfNotNull((TargetZoneObjectModule targetZoneObjectModule) =>
            {
                if (InteractiveObjectInitializationObject.TargetZoneInherentData == null) { targetZoneObjectModule.Init(this.GetModule<LevelCompletionTriggerModule>()); }
                else { targetZoneObjectModule.Init(this.GetModule<LevelCompletionTriggerModule>(), InteractiveObjectInitializationObject.TargetZoneInherentData); }
            });
        }

        private void InitializeAttractiveObjectTypeModule(InteractiveObjectInitializationObject InteractiveObjectInitializationObject)
        {
            this.GetModule<AttractiveObjectTypeModule>().IfNotNull((AttractiveObjectTypeModule attractiveObjectTypeModule) =>
            {
                if (InteractiveObjectInitializationObject.InputAttractiveObjectInherentConfigurationData == null)
                {
                    attractiveObjectTypeModule.Init(this.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[attractiveObjectTypeModule.AttractiveObjectId], this.GetModule<ModelObjectModule>());
                }
                else
                {
                    attractiveObjectTypeModule.Init(InteractiveObjectInitializationObject.InputAttractiveObjectInherentConfigurationData, this.GetModule<ModelObjectModule>());
                }
            }
            );
        }

        private void InitializeProjectileModule(InteractiveObjectInitializationObject InteractiveObjectInitializationObject)
        {
            this.GetModule<LaunchProjectileModule>().IfNotNull((LaunchProjectileModule launchProjectileModule) =>
            {
                if (InteractiveObjectInitializationObject.ProjectilePath != null)
                {
                    if (InteractiveObjectInitializationObject.ProjectileInherentData == null) { launchProjectileModule.Init(this.PuzzleGameConfigurationManager.ProjectileConf()[this.GetModule<LaunchProjectileModule>().LaunchProjectileId], InteractiveObjectInitializationObject.ProjectilePath, this.transform); }
                    else { launchProjectileModule.Init(InteractiveObjectInitializationObject.ProjectileInherentData, InteractiveObjectInitializationObject.ProjectilePath, this.transform); }
                }
            });
        }

        #endregion

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.GetModule<AttractiveObjectTypeModule>().IfNotNull((AttractiveObjectTypeModule attractiveObjectTypeModule) => attractiveObjectTypeModule.Tick(d, timeAttenuationFactor));
            this.GetModule<ObjectRepelTypeModule>().IfNotNull((ObjectRepelTypeModule objectRepelTypeModule) => objectRepelTypeModule.Tick(d, timeAttenuationFactor));
            this.GetModule<LaunchProjectileModule>().IfNotNull((LaunchProjectileModule launchProjectileModule) => launchProjectileModule.Tick(d, timeAttenuationFactor));
        }

        public void DisableModule(Type moduleType)
        {
            this.GetModule(moduleType).IfNotNull((m) =>
            {
                m.gameObject.SetActive(false);
                this.enabledModules.Remove(moduleType);
                this.disabledModules[moduleType] = m;
                this.InteractiveObjectContainer.OnModuleDisabled(m);
            });
        }

        private void EnableModuleOnInit(InteractiveObjectModule moduleToEnable)
        {
            this.enabledModules[moduleToEnable.GetType()] = moduleToEnable;
            this.InteractiveObjectContainer.OnModuleEnabled(moduleToEnable);
        }

        private void EnableModule(Type moduleType)
        {
            this.GetDisabledModule(moduleType).IfNotNull((m) =>
            {
                m.gameObject.SetActive(true);
                this.disabledModules.Remove(moduleType);
                this.enabledModules[moduleType] = m;
                this.InteractiveObjectContainer.OnModuleEnabled(m);
            });

        }

        internal void EnableAllDisabledModules(InteractiveObjectInitializationObject InteractiveObjectInitializationObject)
        {
            foreach (var disabledModule in this.disabledModules.Keys.ToList())
            {
                this.EnableModule(disabledModule);
                if(disabledModule == typeof(ModelObjectModule)) { this.InitializeModelObjectModule(); }
                else if(disabledModule == typeof(AttractiveObjectTypeModule)) { this.InitializeAttractiveObjectTypeModule(InteractiveObjectInitializationObject); }
                else if (disabledModule == typeof(ObjectRepelTypeModule)) { this.IntitializeObjectRepelTypeModule(); }
                else if (disabledModule == typeof(LevelCompletionTriggerModule)) { this.InitializeLevelCompletionTriggerModule(); }
                else if (disabledModule == typeof(TargetZoneObjectModule)) { this.InitializeTargetZoneObjectModule(InteractiveObjectInitializationObject); }
                else if (disabledModule == typeof(LaunchProjectileModule)) { this.InitializeProjectileModule(InteractiveObjectInitializationObject); }
            }
        }

        public void EnableProjectileModule(InteractiveObjectInitializationObject InteractiveObjectInitializationObject)
        {
            this.EnableModule(typeof(LaunchProjectileModule));
            this.InitializeProjectileModule(InteractiveObjectInitializationObject);
        }

        public void EnableAttractiveObjectTypeModule()
        {
            this.EnableModule(typeof(AttractiveObjectTypeModule));
            this.InitializeAttractiveObjectTypeModule(new InteractiveObjectInitializationObject());
        }
    }

    public class InteractiveObjectInitializationObject
    {
        public AttractiveObjectInherentConfigurationData InputAttractiveObjectInherentConfigurationData;
        public TargetZoneInherentData TargetZoneInherentData;
        public ProjectileInherentData ProjectileInherentData;
        public BeziersControlPoints ProjectilePath;

        public InteractiveObjectInitializationObject(AttractiveObjectInherentConfigurationData InputAttractiveObjectInherentConfigurationData = null,
            TargetZoneInherentData TargetZoneInherentData = null, ProjectileInherentData ProjectileInherentData = null, BeziersControlPoints ProjectilePath = null)
        {
            this.InputAttractiveObjectInherentConfigurationData = InputAttractiveObjectInherentConfigurationData;
            this.TargetZoneInherentData = TargetZoneInherentData;
            this.ProjectileInherentData = ProjectileInherentData;
            this.ProjectilePath = ProjectilePath;
        }
    }

}
