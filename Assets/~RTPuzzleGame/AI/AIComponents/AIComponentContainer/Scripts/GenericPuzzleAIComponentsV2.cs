using System;
using System.Collections.Generic;

namespace RTPuzzle
{
    [System.Serializable]
    public class GenericPuzzleAIComponentsV2 : AbstractObjectDefinitionConfigurationInherentData
    {
        public override List<Type> ModuleTypes => new List<Type>()
        {
            typeof(AIPatrolComponent),
            typeof(AIProjectileEscapeComponent),
            typeof(AIEscapeWithoutTriggerComponent),
            typeof(AITargetZoneComponent),
            typeof(AIAttractiveObjectComponent),
            typeof(AIFearStunComponent),
            typeof(AIPlayerEscapeComponent),
            typeof(AIMoveTowardPlayerComponent),
            typeof(AIDisarmObjectComponent),
        };
    }
}

