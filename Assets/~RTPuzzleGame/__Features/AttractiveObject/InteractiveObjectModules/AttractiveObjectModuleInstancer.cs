using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GameConfigurationID;

namespace RTPuzzle
{
    public static class AttractiveObjectModuleInstancer
    {
        public static void PopuplateFromDefinition(AttractiveObjectModuleDefinition attractiveObjectModuleDefinition, Transform parent)
        {
            var puzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            var attractiveObjectModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseAttractiveObjectModule, parent);
            attractiveObjectModule.AttractiveObjectId = attractiveObjectModuleDefinition.AttractiveObjectId;
            var RangeTypeObject = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseRangeTypeObject, attractiveObjectModule.transform);
            new RangeTypeObjectDefinitionInherentData()
            {
                RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                    {
                        { typeof(RangeTypeDefinition),  new RangeTypeDefinition()
                            {
                                RangeTypeID = RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
                                RangeShapeConfiguration = new SphereRangeShapeConfiguration()
                                {
                                    Radius = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[attractiveObjectModule.AttractiveObjectId].EffectRange
                                }
                            }
                        },
                        {typeof(RangeObstacleListenerDefinition), new RangeObstacleListenerDefinition() }
                    },
                RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                    {
                        {typeof(RangeTypeDefinition), true },
                        {typeof(RangeObstacleListenerDefinition), true }
                    }
            }.DefineRangeTypeObject(RangeTypeObject, puzzlePrefabConfiguration);
            RangeTypeObject.RangeTypeObjectDefinitionID = RangeTypeObjectDefinitionID.NONE;
        }
    }
}
