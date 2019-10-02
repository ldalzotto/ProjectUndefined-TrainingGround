using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public static class TargetZoneObjectDefinition
    {
        public static InteractiveObjectInitialization TargetZone(InteractiveObjectTestID interactiveObjectTestID,
            TargetZoneInherentData TargetZoneInherentData)
        {
            var InteractiveObjectInitialization = new InteractiveObjectInitialization()
            {
                InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                {
                    RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()  {

                   {typeof(LevelCompletionTriggerModuleDefinition),
                            new LevelCompletionTriggerModuleDefinition(){
                                RangeObjectInitialization = RangeObjectInitializationDataBuilderV2.BoxRangeNoObstacleListener(Vector3.zero, Vector3.zero, RangeTypeID.TARGET_ZONE)
                            }
                        },
                        {typeof(TargetZoneModuleDefinition), new TargetZoneModuleDefinition(){ TargetZoneID =InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].TargetZoneID  } }
                    },
                    RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                    {
                        { typeof(LevelCompletionTriggerModuleDefinition), true },
                        {typeof(TargetZoneModuleDefinition), true }
                    }
                },
                InteractiveObjectInitializationObject = new InteractiveObjectInitializationObject()
                {
                    TargetZoneInherentData = TargetZoneInherentData
                }
            };

            InteractiveObjectInitialization.InitializeTestConfigurations(interactiveObjectTestID);
            return InteractiveObjectInitialization;
        }

    }
}
