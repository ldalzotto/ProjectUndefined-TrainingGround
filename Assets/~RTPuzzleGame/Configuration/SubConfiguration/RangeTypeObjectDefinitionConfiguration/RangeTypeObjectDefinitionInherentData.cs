using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RangeTypeObjectDefinitionInherentData", menuName = "Configuration/PuzzleGame/RangeTypeObjectDefinitionConfiguration/RangeTypeObjectDefinitionInherentData", order = 1)]
    public class RangeTypeObjectDefinitionInherentData : AbstractObjectDefinitionConfigurationInherentData
    {
        public static List<Type> RangeModuleTypes = new List<Type>() { typeof(RangeTypeDefinition), typeof(RangeObstacleListenerDefinition) };

        public override List<Type> ModuleTypes => RangeTypeObjectDefinitionInherentData.RangeModuleTypes;

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
        public static RangeTypeObjectDefinitionInherentData SphereRangeWithObstacleListener(float sphereRange, RangeTypeID rangeTypeID)
        {
            return new RangeTypeObjectDefinitionInherentData()
            {
                RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                {
                    {typeof(RangeTypeDefinition), new RangeTypeDefinition(){
                        RangeTypeID = rangeTypeID,
                        RangeShapeConfiguration = new SphereRangeShapeConfiguration()
                        {
                            Radius = sphereRange
                        }
                    } },
                    {typeof(RangeObstacleListenerDefinition), new RangeObstacleListenerDefinition() }
                },
                RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                {
                    {typeof(RangeTypeDefinition), true },
                    {typeof(RangeObstacleListenerDefinition), true }
                }
            };
        }
        
        public static RangeTypeObjectDefinitionInherentData BoxRangeNoObstacleListener(Vector3 center, Vector3 size, RangeTypeID rangeTypeID)
        {
            return new RangeTypeObjectDefinitionInherentData()
            {
                RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                {
                    {typeof(RangeTypeDefinition), new RangeTypeDefinition(){
                        RangeTypeID = rangeTypeID,
                        RangeShapeConfiguration = new BoxRangeShapeConfiguration()
                        {
                            Center = center,
                            Size = size
                        }
                    } }
                },
                RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                {
                    {typeof(RangeTypeDefinition), true }
                }
            };
        }
    }

}
