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
        #endregion

        #region Data Retrieval
        public ModelObjectModule ModelObjectModule { get => modelObjectModule; }
        public AttractiveObjectTypeModule AttractiveObjectTypeModule { get => attractiveObjectTypeModule; }
        public ObjectRepelTypeModule ObjectRepelTypeModule { get => objectRepelTypeModule; set => objectRepelTypeModule = value; }
        #endregion

        public void Init(AttractiveObjectInherentConfigurationData InputAttractiveObjectInherentConfigurationData = null)
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
                }
            }

            this.ModelObjectModule.IfNotNull((ModelObjectModule modelObjectModule) => modelObjectModule.Init());
            this.AttractiveObjectTypeModule.IfNotNull((AttractiveObjectTypeModule attractiveObjectTypeModule) =>
            {
                if (InputAttractiveObjectInherentConfigurationData == null)
                {
                    attractiveObjectTypeModule.Init(puzzleGameConfigurationManager.AttractiveObjectsConfiguration()[attractiveObjectTypeModule.AttractiveObjectId], this.ModelObjectModule);
                } else
                {
                    attractiveObjectTypeModule.Init(InputAttractiveObjectInherentConfigurationData, this.ModelObjectModule);
                }
            }
            );
            this.ObjectRepelTypeModule.IfNotNull((ObjectRepelTypeModule objectRepelTypeModule) => objectRepelTypeModule.Init(this.ModelObjectModule));

            interactiveObjectContainer.OnInteractiveObjectAdded(this);
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectTypeModule.IfNotNull((AttractiveObjectTypeModule attractiveObjectTypeModule) => attractiveObjectTypeModule.Tick(d, timeAttenuationFactor));
            this.ObjectRepelTypeModule.IfNotNull((ObjectRepelTypeModule objectRepelTypeModule) => objectRepelTypeModule.Tick(d, timeAttenuationFactor));
        }

    }

}
