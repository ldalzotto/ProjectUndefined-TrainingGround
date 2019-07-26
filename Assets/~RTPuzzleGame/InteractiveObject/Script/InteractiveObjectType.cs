using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class InteractiveObjectType : MonoBehaviour
    {

        #region Modules
        private ModelObjectModule modelObjectModule;
        private AttractiveObjectTypeModule attractiveObjectTypeModule;
        private ObjectRepelTypeModule objectRepelTypeModule;
        private LevelCompletionTriggerModule levelCompletionTriggerModule;
        private TargetZoneObjectModule targetZoneObjectModule;
        private LaunchProjectileModule launchProjectileModule;
        #endregion

        #region External Dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion


        #region Data Retrieval
        public ModelObjectModule ModelObjectModule { get => modelObjectModule; }
        public AttractiveObjectTypeModule AttractiveObjectTypeModule { get => attractiveObjectTypeModule; }
        public ObjectRepelTypeModule ObjectRepelTypeModule { get => objectRepelTypeModule; }
        public LevelCompletionTriggerModule LevelCompletionTriggerModule { get => levelCompletionTriggerModule; }
        public TargetZoneObjectModule TargetZoneObjectModule { get => targetZoneObjectModule; }
        public LaunchProjectileModule LaunchProjectileModule { get => launchProjectileModule; }
        #endregion

        public void Init(InteractiveObjectInitializationObject InteractiveObjectInitializationObject)
        {
            #region External Dependencies
            this.PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            #endregion

            this.PopulateModules();

            this.ModelObjectModule.IfNotNull((ModelObjectModule modelObjectModule) => modelObjectModule.Init());
            this.AttractiveObjectTypeModule.IfNotNull((AttractiveObjectTypeModule attractiveObjectTypeModule) =>
            {
                if (InteractiveObjectInitializationObject.InputAttractiveObjectInherentConfigurationData == null)
                {
                    attractiveObjectTypeModule.Init(this.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[attractiveObjectTypeModule.AttractiveObjectId], this.ModelObjectModule);
                }
                else
                {
                    attractiveObjectTypeModule.Init(InteractiveObjectInitializationObject.InputAttractiveObjectInherentConfigurationData, this.ModelObjectModule);
                }
            }
            );
            this.ObjectRepelTypeModule.IfNotNull((ObjectRepelTypeModule objectRepelTypeModule) => objectRepelTypeModule.Init(this.ModelObjectModule));
            this.LevelCompletionTriggerModule.IfNotNull((LevelCompletionTriggerModule levelCompletionTriggerModule) => levelCompletionTriggerModule.Init());
            this.TargetZoneObjectModule.IfNotNull((TargetZoneObjectModule targetZoneObjectModule) =>
            {
                if (InteractiveObjectInitializationObject.TargetZoneInherentData == null) { targetZoneObjectModule.Init(this.levelCompletionTriggerModule); }
                else { targetZoneObjectModule.Init(this.levelCompletionTriggerModule, InteractiveObjectInitializationObject.TargetZoneInherentData); }
            });
            InitializeProjectileModule(InteractiveObjectInitializationObject);

            interactiveObjectContainer.OnInteractiveObjectAdded(this);
        }

        private void InitializeProjectileModule(InteractiveObjectInitializationObject InteractiveObjectInitializationObject)
        {
            this.LaunchProjectileModule.IfNotNull((LaunchProjectileModule launchProjectileModule) =>
            {
                if (InteractiveObjectInitializationObject.ProjectilePath != null)
                {
                    if (InteractiveObjectInitializationObject.ProjectileInherentData == null) { launchProjectileModule.Init(this.PuzzleGameConfigurationManager.ProjectileConf()[this.launchProjectileModule.LaunchProjectileId], InteractiveObjectInitializationObject.ProjectilePath, this.transform); }
                    else { launchProjectileModule.Init(InteractiveObjectInitializationObject.ProjectileInherentData, InteractiveObjectInitializationObject.ProjectilePath, this.transform); }
                }
            });
        }

        private void PopulateModules()
        {
            var retrievedInteractiveObjectModules = this.GetComponentsInChildren<InteractiveObjectModule>();
            if (retrievedInteractiveObjectModules != null)
            {
                foreach (var interactiveObjectModule in retrievedInteractiveObjectModules)
                {
                    interactiveObjectModule.IfTypeEqual((AttractiveObjectTypeModule interactiveObjectModule2) => this.attractiveObjectTypeModule = interactiveObjectModule2);
                    interactiveObjectModule.IfTypeEqual((ModelObjectModule interactiveObjectModule2) => this.modelObjectModule = interactiveObjectModule2);
                    interactiveObjectModule.IfTypeEqual((ObjectRepelTypeModule interactiveObjectModule2) => this.objectRepelTypeModule = interactiveObjectModule2);
                    interactiveObjectModule.IfTypeEqual((LevelCompletionTriggerModule interactiveObjectModule2) => this.levelCompletionTriggerModule = interactiveObjectModule2);
                    interactiveObjectModule.IfTypeEqual((TargetZoneObjectModule interactiveObjectModule2) => this.targetZoneObjectModule = interactiveObjectModule2);
                    interactiveObjectModule.IfTypeEqual((LaunchProjectileModule launchProjectileModule2) => this.launchProjectileModule = launchProjectileModule2);
                }
            }
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectTypeModule.IfNotNull((AttractiveObjectTypeModule attractiveObjectTypeModule) => attractiveObjectTypeModule.Tick(d, timeAttenuationFactor));
            this.ObjectRepelTypeModule.IfNotNull((ObjectRepelTypeModule objectRepelTypeModule) => objectRepelTypeModule.Tick(d, timeAttenuationFactor));
            this.LaunchProjectileModule.IfNotNull((LaunchProjectileModule launchProjectileModule) => launchProjectileModule.Tick(d, timeAttenuationFactor));
        }

        public void DisableModule<T>(T module) where T : InteractiveObjectModule
        {
            module.gameObject.SetActive(false);
        }

        public void EnableProjectileModule(InteractiveObjectInitializationObject InteractiveObjectInitializationObject)
        {
            this.launchProjectileModule.gameObject.SetActive(true);
            this.InitializeProjectileModule(InteractiveObjectInitializationObject);
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
