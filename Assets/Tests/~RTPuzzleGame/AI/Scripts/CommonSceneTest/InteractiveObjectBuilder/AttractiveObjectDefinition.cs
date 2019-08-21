using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public static class AttractiveObjectDefinition
    {
        public static InteractiveObjectInitialization AttractiveObjectOnly(
             InteractiveObjectTestID interactiveObjectTestID,
             float attractiveObjectEffectRange, float attractiveObjectEffectiveTime)
        {
            var InteractiveObjectInitialization =
                  new InteractiveObjectInitialization()
                  {
                      InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                      {
                          RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                        {
                            {typeof(AttractiveObjectModuleDefinition), new AttractiveObjectModuleDefinition(){ AttractiveObjectId = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].AttractiveObjectId }},
                            {typeof(ModelObjectModuleDefinition), new ModelObjectModuleDefinition() }
                        },
                          RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                        {
                            {typeof(AttractiveObjectModuleDefinition), true },
                            {typeof(ModelObjectModuleDefinition), true }
                        }
                      },
                      InteractiveObjectInitializationObject = new InteractiveObjectInitializationObject()
                      {
                          AttractiveObjectInherentConfigurationData = new AttractiveObjectInherentConfigurationData()
                          {
                              EffectiveTime = attractiveObjectEffectiveTime,
                              EffectRange = attractiveObjectEffectRange
                          }
                      }
                  };
            InteractiveObjectInitialization.InitializeTestConfigurations(interactiveObjectTestID);
            return InteractiveObjectInitialization;
        }
    }
}
