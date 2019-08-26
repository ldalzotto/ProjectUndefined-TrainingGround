using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public static class RangeObjectDefinition
    {
        public static RangeObjectInitialization SphereObstacleListening(RangeTypeID rangeTypeID, float radius)
        {
            var RangeObjectInitialization = new RangeObjectInitialization()
            {
                RangeTypeObjectDefinitionInherentData = new RangeTypeObjectDefinitionInherentData()
                {
                    RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                    {
                        {typeof(RangeTypeDefinition), new RangeTypeDefinition(){
                            RangeTypeID = rangeTypeID,
                            RangeShapeConfiguration = new SphereRangeShapeConfiguration(){
                                                            Radius = radius
                                                      }
                        }},
                        {typeof(RangeObstacleListenerDefinition), new RangeObstacleListenerDefinition() }
                    },
                    RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                    {
                        {typeof(RangeTypeDefinition), true},
                        {typeof(RangeObstacleListenerDefinition), true }
                    }
                }
            };
            return RangeObjectInitialization;
        }
    }

}
