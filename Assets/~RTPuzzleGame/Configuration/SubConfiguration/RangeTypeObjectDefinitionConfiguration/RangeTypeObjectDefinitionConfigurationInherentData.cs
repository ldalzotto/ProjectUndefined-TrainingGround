using GameConfigurationID;
using OdinSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RangeTypeObjectDefinitionConfigurationInherentData", menuName = "Configuration/PuzzleGame/RangeTypeObjectDefinitionConfiguration/RangeTypeObjectDefinitionConfigurationInherentData", order = 1)]
    public class RangeTypeObjectDefinitionConfigurationInherentData : AbstractObjectDefinitionConfigurationInherentData
    {
        public static List<Type> RangeModuleTypes = new List<Type>() { typeof(RangeTypeDefinition), typeof(RangeObstacleListenerDefinition) };

        public override List<Type> ModuleTypes => RangeTypeObjectDefinitionConfigurationInherentData.RangeModuleTypes;

        public void DefineRangeTypeObject(RangeTypeObject rangeTypeObject, PuzzlePrefabConfiguration puzzlePrefabConfiguration)
        {
            if (this.RangeDefinitionModulesActivation != null && this.RangeDefinitionModules != null)
            {
                this.DestroyExistingModules(rangeTypeObject.gameObject);

                foreach (var rangeDefinitionModuleActivation in this.RangeDefinitionModulesActivation)
                {
                    if (rangeDefinitionModuleActivation.Value)
                    {
                        var moduleConfiguration = this.RangeDefinitionModules[rangeDefinitionModuleActivation.Key];
                        if (moduleConfiguration.GetType() == typeof(RangeTypeDefinition))
                        {
                            var RangeTypeDefinition = (RangeTypeDefinition)moduleConfiguration;
                            if (RangeTypeDefinition.RangeShapeConfiguration != null)
                            {
                                if (RangeTypeDefinition.RangeShapeConfiguration.GetType() == typeof(FrustumRangeShapeConfiguration))
                                {
                                    var frustumRangeType = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseFrustumRangeType, rangeTypeObject.transform);
                                    frustumRangeType.PopulateFromDefinition(RangeTypeDefinition);
                                }
                                else if (RangeTypeDefinition.RangeShapeConfiguration.GetType() == typeof(RoundedFrustumRangeShapeConfiguration))
                                {
                                    var roundedFrustumRangeType = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseRoundedFrustumRangeType, rangeTypeObject.transform);
                                    roundedFrustumRangeType.PopulateFromDefinition(RangeTypeDefinition);
                                }
                                else if (RangeTypeDefinition.RangeShapeConfiguration.GetType() == typeof(BoxRangeShapeConfiguration))
                                {
                                    var boxGrustumRangeType = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseBoxRangeType, rangeTypeObject.transform);
                                    boxGrustumRangeType.PopulateFromDefinition(RangeTypeDefinition);
                                }
                                else if (RangeTypeDefinition.RangeShapeConfiguration.GetType() == typeof(SphereRangeShapeConfiguration))
                                {
                                    var sphereRangeType = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseSphereRangeType, rangeTypeObject.transform);
                                    sphereRangeType.PopulateFromDefinition(RangeTypeDefinition);
                                }
                            }
                        }
                        else if (moduleConfiguration.GetType() == typeof(RangeObstacleListenerDefinition))
                        {
                            MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseRangeObstacleListener, rangeTypeObject.transform);
                        }
                    }
                }
            }
        }
    }

    public static class RangeTypeObjectDefinitionConfigurationInherentDataBuilder
    {
        public static RangeTypeObjectDefinitionConfigurationInherentData SphereRangeWithObstacleListener(float sphereRange, RangeTypeID rangeTypeID, RangeTypeObjectDefinitionConfiguration rangeTypeObjectDefinitionConfiguration)
        {
            RangeTypeObjectDefinitionConfigurationInherentData RangeTypeObjectDefinitionConfigurationInherentData =
                    GameObject.Instantiate(rangeTypeObjectDefinitionConfiguration.ConfigurationInherentData[RangeTypeObjectDefinitionID.INPUT_SphereRange_WithObstacleListener]);
            var RangeTypeDefinition = (RangeTypeDefinition)RangeTypeObjectDefinitionConfigurationInherentData.RangeDefinitionModules[typeof(RangeTypeDefinition)];
            RangeTypeDefinition.RangeTypeID = rangeTypeID;
            ((SphereRangeShapeConfiguration)RangeTypeDefinition.RangeShapeConfiguration).Radius = sphereRange;
            return RangeTypeObjectDefinitionConfigurationInherentData;
        }
    }
}
