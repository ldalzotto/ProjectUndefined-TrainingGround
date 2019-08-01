using System;
using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public class PointOfInterestModules : MonoBehaviour
    {

        private PointOfInterestModelObjectModule pointOfInterestModelObjectModule;
        private PointOfInterestCutsceneControllerModule pointOfInterestCutsceneController;
        private PointOfInterestTrackerModule pointOfInterestTrackerModule;
        private PointOfInterestVisualMovementModule pointOfInterestVisualMovementModule;

        #region Data Retrieval
        public PointOfInterestModelObjectModule PointOfInterestModelObjectModule { get => pointOfInterestModelObjectModule; }
        public PointOfInterestCutsceneControllerModule PointOfInterestCutsceneController { get => pointOfInterestCutsceneController; }
        public PointOfInterestTrackerModule PointOfInterestTrackerModule { get => pointOfInterestTrackerModule; }
        public PointOfInterestVisualMovementModule PointOfInterestVisualMovementModule { get => pointOfInterestVisualMovementModule; }
        #endregion
        
        public void Init(PointOfInterestType pointOfInterestTypeRef)
        {
            var retrievedPointOfInterestModules = this.transform.GetComponentsInChildren<APointOfInterestModule>();
            if (retrievedPointOfInterestModules != null)
            {
                foreach (var retrievedPointOfInterestModule in retrievedPointOfInterestModules)
                {
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestModelObjectModule retrievedPointOfInterestModule2) => this.pointOfInterestModelObjectModule = retrievedPointOfInterestModule2);
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestCutsceneControllerModule retrievedPointOfInterestModule2) => this.pointOfInterestCutsceneController = retrievedPointOfInterestModule2);
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestTrackerModule retrievedPointOfInterestModule2) => this.pointOfInterestTrackerModule = retrievedPointOfInterestModule2);
                    retrievedPointOfInterestModule.IfTypeEqual((PointOfInterestVisualMovementModule retrievedPointOfInterestModule2) => this.pointOfInterestVisualMovementModule = retrievedPointOfInterestModule2);
                }
            }

            this.pointOfInterestModelObjectModule.IfNotNull((pointOfInterestModelObjectModule) => pointOfInterestModelObjectModule.Init(pointOfInterestTypeRef, this.pointOfInterestModelObjectModule));
            this.pointOfInterestCutsceneController.IfNotNull((pointOfInterestCutsceneController) => pointOfInterestCutsceneController.Init(pointOfInterestTypeRef, this.pointOfInterestModelObjectModule));
            this.pointOfInterestTrackerModule.IfNotNull((pointOfInterestTrackerModule) => pointOfInterestTrackerModule.Init(pointOfInterestTypeRef));
            this.pointOfInterestVisualMovementModule.IfNotNull((pointOfInterestVisualMovementModule) => pointOfInterestVisualMovementModule.Init(pointOfInterestTypeRef, this.pointOfInterestModelObjectModule, this.pointOfInterestTrackerModule));
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
