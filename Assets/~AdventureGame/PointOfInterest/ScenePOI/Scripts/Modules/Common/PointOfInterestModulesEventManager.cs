using CoreGame;

namespace AdventureGame
{
    public class PointOfInterestModulesEventManager
    {
        private PointOfInterestModules PointOfInterestModules;

        public PointOfInterestModulesEventManager(PointOfInterestModules pointOfInterestModules)
        {
            PointOfInterestModules = pointOfInterestModules;
        }

        #region External Events
        public void OnPOIInit()
        {
            this.PointOfInterestModules.PointOfInterestModelObjectModule.IfNotNull((pointOfInterestModelObjectModule) => pointOfInterestModelObjectModule.OnPOIInit());
        }

        internal void OnPOIDisabled(APointOfInterestType pointOfInterestType)
        {
            this.PointOfInterestModules.PointOfInterestTrackerModule.IfNotNull((pointOfInterestTrackerModule) => pointOfInterestTrackerModule.OnPOIDisabled(pointOfInterestType));
        }
        #endregion
    }

}
