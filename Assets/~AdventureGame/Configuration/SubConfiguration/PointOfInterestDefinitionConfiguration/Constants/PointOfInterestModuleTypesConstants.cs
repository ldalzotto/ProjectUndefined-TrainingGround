using System;
using System.Collections.Generic;

namespace AdventureGame
{
    public class PointOfInterestModuleTypesConstants
    {
        public static List<Type> PointOfInterestModuleTypes = new List<Type>() {
            typeof(PointOfInterestCutsceneControllerModuleDefinition),
            typeof(PointOfInterestTrackerModuleDefinition),
            typeof(PointOfInterestVisualMovementModuleDefinition),
            typeof(PointOfInterestModelObjectModuleDefinition),
            typeof(PointOfInterestLogicColliderModuleDefinition),
//${addNewEntry}
        };
    }

}
