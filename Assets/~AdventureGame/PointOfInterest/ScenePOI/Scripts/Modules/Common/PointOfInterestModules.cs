using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public class PointOfInterestModules : MonoBehaviour
    {

        private PointOfInterestModelObjectModule pointOfInterestModelObjectModule;
        private PointOfInterestCutsceneController pointOfInterestCutsceneController;
        private PointOfInterestTrackerModule pointOfInterestTrackerModule;
        private PointOfInterestVisualMovementModule pointOfInterestVisualMovementModule;
        private PointOfInterestSpecificBehaviorModule pointOfInterestSpecificBehaviorModule;

        #region Data Retrieval
        public PointOfInterestModelObjectModule PointOfInterestModelObjectModule { get => pointOfInterestModelObjectModule; }
        public PointOfInterestCutsceneController PointOfInterestCutsceneController { get => pointOfInterestCutsceneController; }
        public PointOfInterestTrackerModule PointOfInterestTrackerModule { get => pointOfInterestTrackerModule; }
        public PointOfInterestVisualMovementModule PointOfInterestVisualMovementModule { get => pointOfInterestVisualMovementModule; }
        public PointOfInterestSpecificBehaviorModule PointOfInterestSpecificBehaviorModule { get => pointOfInterestSpecificBehaviorModule; }
        #endregion

        public void Init(PointOfInterestType pointOfInterestTypeRef)
        {
            var retrievedPointOfInterestModules = this.transform.GetComponentsInChildren<APointOfInterestModule>();
            if (retrievedPointOfInterestModules != null)
            {
                foreach (var retrievedPointOfInterestModule in retrievedPointOfInterestModules)
                {
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestModelObjectModule retrievedPointOfInterestModule2) => this.pointOfInterestModelObjectModule = retrievedPointOfInterestModule2);
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestCutsceneController retrievedPointOfInterestModule2) => this.pointOfInterestCutsceneController = retrievedPointOfInterestModule2);
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestTrackerModule retrievedPointOfInterestModule2) => this.pointOfInterestTrackerModule = retrievedPointOfInterestModule2);
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestVisualMovementModule retrievedPointOfInterestModule2) => this.pointOfInterestVisualMovementModule = retrievedPointOfInterestModule2);
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestSpecificBehaviorModule retrievedPointOfInterestModule2) => this.pointOfInterestSpecificBehaviorModule = retrievedPointOfInterestModule2);
                }
            }

            this.pointOfInterestModelObjectModule.IfNotNull((pointOfInterestModelObjectModule) => pointOfInterestModelObjectModule.Init(pointOfInterestTypeRef, this.pointOfInterestModelObjectModule));
            this.pointOfInterestCutsceneController.IfNotNull((pointOfInterestCutsceneController) => pointOfInterestCutsceneController.Init(pointOfInterestTypeRef, this.pointOfInterestModelObjectModule));
            this.pointOfInterestTrackerModule.IfNotNull((pointOfInterestTrackerModule) => pointOfInterestTrackerModule.Init(pointOfInterestTypeRef));
            this.pointOfInterestVisualMovementModule.IfNotNull((pointOfInterestVisualMovementModule) => pointOfInterestVisualMovementModule.Init(pointOfInterestTypeRef, this.pointOfInterestModelObjectModule, this.pointOfInterestTrackerModule));
            this.pointOfInterestSpecificBehaviorModule.IfNotNull((pointOfInterestSpecificBehaviorModule) => pointOfInterestSpecificBehaviorModule.Init(pointOfInterestTypeRef, this.pointOfInterestModelObjectModule));
        }

        public void Tick(float d)
        {
            this.pointOfInterestCutsceneController.IfNotNull((pointOfInterestCutsceneController) => pointOfInterestCutsceneController.Tick(d));
            this.pointOfInterestTrackerModule.IfNotNull((pointOfInterestTrackerModule) => pointOfInterestTrackerModule.Tick(d));
            this.pointOfInterestSpecificBehaviorModule.IfNotNull((pointOfInterestSpecificBehaviorModule) => pointOfInterestSpecificBehaviorModule.Tick(d));
        }

        public void LateTick(float d)
        {
            this.pointOfInterestVisualMovementModule.IfNotNull((pointOfInterestVisualMovementModule) => pointOfInterestVisualMovementModule.LateTick(d));
        }
    }

}
