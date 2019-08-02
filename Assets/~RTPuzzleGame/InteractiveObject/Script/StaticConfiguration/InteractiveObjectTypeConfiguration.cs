using System;
using System.Collections.Generic;

namespace RTPuzzle
{
    public static class InteractiveObjectTypeConfiguration
    {
        public static Dictionary<Type, Action<InteractiveObjectInitializationObject, InteractiveObjectType>> InitializationConfiguration = new Dictionary<Type, Action<InteractiveObjectInitializationObject, InteractiveObjectType>>()
        {
            { typeof(ModelObjectModule), InteractiveObjectModulesInitializationOperations.InitializeModelObjectModule },
            {typeof(AttractiveObjectTypeModule), InteractiveObjectModulesInitializationOperations.InitializeAttractiveObjectTypeModule },
            {typeof(ObjectRepelTypeModule), InteractiveObjectModulesInitializationOperations.IntitializeObjectRepelTypeModule },
            {typeof(LevelCompletionTriggerModule), InteractiveObjectModulesInitializationOperations.InitializeLevelCompletionTriggerModule },
            {typeof(TargetZoneObjectModule), InteractiveObjectModulesInitializationOperations.InitializeTargetZoneObjectModule },
            {typeof(LaunchProjectileModule), InteractiveObjectModulesInitializationOperations.InitializeProjectileModule },
            {typeof(DisarmObjectModule), InteractiveObjectModulesInitializationOperations.InitializeDisarmObjectModule },
            {typeof(ActionInteractableObjectModule), InteractiveObjectModulesInitializationOperations.InitializeActionInteractableObjectModule },
        };
    }
}
