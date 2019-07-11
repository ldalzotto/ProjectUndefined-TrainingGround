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
        #endregion

        #region Data Retrieval
        public ModelObjectModule ModelObjectModule { get => modelObjectModule; }
        public AttractiveObjectTypeModule AttractiveObjectTypeModule { get => attractiveObjectTypeModule; }
        public ObjectRepelTypeModule ObjectRepelTypeModule { get => objectRepelTypeModule; }
        public LevelCompletionTriggerModule LevelCompletionTriggerModule { get => levelCompletionTriggerModule; }
        public TargetZoneObjectModule TargetZoneObjectModule { get => targetZoneObjectModule; }
        #endregion

        public void Init(AttractiveObjectInherentConfigurationData InputAttractiveObjectInherentConfigurationData = null,
                                TargetZoneInherentData targetZoneInherentData = null)
        {
            #region External Dependencies
            var puzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var interactiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            #endregion

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
                }
            }

            this.ModelObjectModule.IfNotNull((ModelObjectModule modelObjectModule) => modelObjectModule.Init());
            this.AttractiveObjectTypeModule.IfNotNull((AttractiveObjectTypeModule attractiveObjectTypeModule) =>
                {
                    if (InputAttractiveObjectInherentConfigurationData == null)
                    {
                        attractiveObjectTypeModule.Init(puzzleGameConfigurationManager.AttractiveObjectsConfiguration()[attractiveObjectTypeModule.AttractiveObjectId], this.ModelObjectModule);
                    }
                    else
                    {
                        attractiveObjectTypeModule.Init(InputAttractiveObjectInherentConfigurationData, this.ModelObjectModule);
                    }
                }
            );
            this.ObjectRepelTypeModule.IfNotNull((ObjectRepelTypeModule objectRepelTypeModule) => objectRepelTypeModule.Init(this.ModelObjectModule));
            this.LevelCompletionTriggerModule.IfNotNull((LevelCompletionTriggerModule levelCompletionTriggerModule) => levelCompletionTriggerModule.Init());
            this.TargetZoneObjectModule.IfNotNull((TargetZoneObjectModule targetZoneObjectModule) =>
            {
                if (targetZoneInherentData == null) { targetZoneObjectModule.Init(this.levelCompletionTriggerModule); }
                else { targetZoneObjectModule.Init(this.levelCompletionTriggerModule, targetZoneInherentData); }
            });

            interactiveObjectContainer.OnInteractiveObjectAdded(this);
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectTypeModule.IfNotNull((AttractiveObjectTypeModule attractiveObjectTypeModule) => attractiveObjectTypeModule.Tick(d, timeAttenuationFactor));
            this.ObjectRepelTypeModule.IfNotNull((ObjectRepelTypeModule objectRepelTypeModule) => objectRepelTypeModule.Tick(d, timeAttenuationFactor));
        }

    }

}
