using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public static class LevelCompletionTriggerDefinition
    {
        public static InteractiveObjectInitialization LevelCOmpletionTargetZone(
                 InteractiveObjectTestID interactiveObjectTestID,
                 Vector3 completionBoxLocalCenter, Vector3 completionBoxLocalSize,
                 float targetZoneAiDistanceDetection, float targetZoneEscapeFOVSemiAngle
            )
        {
            var InteractiveObjectInitialization =
                  new InteractiveObjectInitialization()
                  {
                      InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                      {
                          RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                        {
                            {typeof(LevelCompletionTriggerModuleDefinition), new LevelCompletionTriggerModuleDefinition(){
                               RangeTypeObjectDefinitionIDPicker = false,
                               RangeTypeObjectDefinitionInherentData = new RangeTypeObjectDefinitionInherentData(){
                                   RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                                   {
                                       {typeof(RangeTypeDefinition), new RangeTypeDefinition() { RangeTypeID = RangeTypeID.TARGET_ZONE, RangeShapeConfiguration = new BoxRangeShapeConfiguration(){
                                            Center = completionBoxLocalCenter,
                                            Size = completionBoxLocalSize
                                       } } }
                                   },
                                   RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                                   {
                                       {typeof(RangeTypeDefinition), true }
                                   }
                               } }
                            },
                            {typeof(TargetZoneModuleDefinition), new TargetZoneModuleDefinition(){ TargetZoneID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].TargetZoneID } }
                        },
                          RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                        {
                            {typeof(LevelCompletionTriggerModuleDefinition), true },
                            {typeof(TargetZoneModuleDefinition), true }
                        }
                      },
                      InteractiveObjectInitializationObject = new InteractiveObjectInitializationObject()
                      {
                          TargetZoneInherentData = new TargetZoneInherentData(targetZoneAiDistanceDetection, targetZoneEscapeFOVSemiAngle)
                      }
                  };
            InteractiveObjectInitialization.InitializeTestConfigurations(interactiveObjectTestID);
            return InteractiveObjectInitialization;
        }
        
    }
}
