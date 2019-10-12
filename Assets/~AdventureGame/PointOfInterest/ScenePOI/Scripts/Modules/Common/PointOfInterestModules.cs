using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public class PointOfInterestModules
    {

        private PointOfInterestModelObjectModule pointOfInterestModelObjectModule;
        private PointOfInterestCutsceneControllerModule pointOfInterestCutsceneController;
        private PointOfInterestTrackerModule pointOfInterestTrackerModule;
        private PointOfInterestVisualMovementModule pointOfInterestVisualMovementModule;
        private PointOfInterestLogicColliderModule pointOfInterestLogicColliderModule;

        #region Data Retrieval
        public PointOfInterestModelObjectModule PointOfInterestModelObjectModule { get => pointOfInterestModelObjectModule; }
        public PointOfInterestCutsceneControllerModule PointOfInterestCutsceneController { get => pointOfInterestCutsceneController; }
        public PointOfInterestTrackerModule PointOfInterestTrackerModule { get => pointOfInterestTrackerModule; }
        public PointOfInterestVisualMovementModule PointOfInterestVisualMovementModule { get => pointOfInterestVisualMovementModule; }
        public PointOfInterestLogicColliderModule PointOfInterestLogicColliderModule { get => pointOfInterestLogicColliderModule; }
        #endregion

        public PointOfInterestModules(PointOfInterestType pointOfInterestTypeRef, PointOfInterestDefinitionInherentData PointOfInterestDefinitionInherentData,
            PointOfInterestInitializationObject PointOfInterestInitializationObject = null)
        {
            var AdventureConfigurationManager = AdventureGameSingletonInstances.AdventureGameConfigurationManager;

            var retrievedPointOfInterestModules = pointOfInterestTypeRef.GetComponentsInChildren<APointOfInterestModule>();
            if (retrievedPointOfInterestModules != null)
            {
                foreach (var retrievedPointOfInterestModule in retrievedPointOfInterestModules)
                {
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestModelObjectModule retrievedPointOfInterestModule2) => this.pointOfInterestModelObjectModule = retrievedPointOfInterestModule2);
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestCutsceneControllerModule retrievedPointOfInterestModule2) => this.pointOfInterestCutsceneController = retrievedPointOfInterestModule2);
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestTrackerModule retrievedPointOfInterestModule2) => this.pointOfInterestTrackerModule = retrievedPointOfInterestModule2);
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestVisualMovementModule retrievedPointOfInterestModule2) => this.pointOfInterestVisualMovementModule = retrievedPointOfInterestModule2);
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestLogicColliderModule retrievedPointOfInterestModule2) => this.pointOfInterestLogicColliderModule = retrievedPointOfInterestModule2);
                }
            }

            this.pointOfInterestModelObjectModule.IfNotNull((pointOfInterestModelObjectModule) => pointOfInterestModelObjectModule.Init(pointOfInterestTypeRef, this.pointOfInterestModelObjectModule, this.pointOfInterestLogicColliderModule));
            this.pointOfInterestCutsceneController.IfNotNull((pointOfInterestCutsceneController) => pointOfInterestCutsceneController.Init(pointOfInterestTypeRef, this.pointOfInterestModelObjectModule));
            this.pointOfInterestTrackerModule.IfNotNull((pointOfInterestTrackerModule) => pointOfInterestTrackerModule.Init(pointOfInterestTypeRef));
            this.pointOfInterestVisualMovementModule.IfNotNull((pointOfInterestVisualMovementModule) => {
                PointOfInterestVisualMovementInherentData PointOfInterestVisualMovementInherentData = null;
                if(PointOfInterestInitializationObject!=null && PointOfInterestInitializationObject.PointOfInterestVisualMovementInherentData != null) { PointOfInterestVisualMovementInherentData = PointOfInterestInitializationObject.PointOfInterestVisualMovementInherentData; }
                else { PointOfInterestVisualMovementInherentData = AdventureConfigurationManager.PointOfInterestVisualMovementConfiguration()[PointOfInterestDefinitionInherentData.GetDefinitionModule<PointOfInterestVisualMovementModuleDefinition>().PointOfInterestVisualMovementID]; }
                pointOfInterestVisualMovementModule.Init(pointOfInterestTypeRef, this.pointOfInterestModelObjectModule, this.pointOfInterestTrackerModule, PlayerPointOfInterestSelectionManager.Get(), PointOfInterestVisualMovementInherentData);
            });
            this.pointOfInterestLogicColliderModule.IfNotNull((pointOfInterestLogicColliderModule) => pointOfInterestLogicColliderModule.Init());
        }

        public void Tick(float d)
        {
            this.pointOfInterestCutsceneController.IfNotNull((pointOfInterestCutsceneController) => pointOfInterestCutsceneController.Tick(d));
            this.pointOfInterestTrackerModule.IfNotNull((pointOfInterestTrackerModule) => pointOfInterestTrackerModule.Tick(d));
        }

        public void LateTick(float d)
        {
            this.pointOfInterestVisualMovementModule.IfNotNull((pointOfInterestVisualMovementModule) => pointOfInterestVisualMovementModule.LateTick(d));
        }
    }

}
