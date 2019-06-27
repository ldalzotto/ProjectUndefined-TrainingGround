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
            foreach (var pointOfInterestModule in this.PointOfInterestModules.GetAllPointOfInterestModules())
            {
                if (pointOfInterestModule.Key == typeof(PointOfInterestModelObjectModule))
                {
                    ((PointOfInterestModelObjectModule)pointOfInterestModule.Value).OnPOIInit();
                }
            }
        }
        #endregion
    }

}
