using AdventureGame;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public static class ContextActionOnlyDefinition
    {
        public static PointOfInterestInitialization SingleDummyPOI(PointOfInterestTestID PointOfInterestTestID)
        {
            var PointOfInterestInitialization = new PointOfInterestInitialization()
            {
                PointOfInterestDefinitionInherentData = new PointOfInterestDefinitionInherentData()
                {
                    PointOfInterestId = PointOfInterestIDTree.PointOfInterestTestIDs[PointOfInterestTestID].PointOfInterestId,
                    PointOfInterestSharedDataTypeInherentData = new PointOfInterestSharedDataTypeInherentData()
                    {
                        InteractionWithPlayerAllowed = true,
                        POIDetectionAngleLimit = 999f,
                        TransformMoveManagerComponent = new CoreGame.TransformMoveManagerComponentV3()
                    }, 
                    RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                    {
                        { typeof(PointOfInterestLogicColliderModuleDefinition), new PointOfInterestLogicColliderModuleDefinition() }
                    },
                    RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                    {
                        { typeof(PointOfInterestLogicColliderModuleDefinition), true }
                    }
                }
            };
            PointOfInterestInitialization.InitializeTestConfigurations(PointOfInterestTestID);
            return PointOfInterestInitialization;
        }
    }

}
