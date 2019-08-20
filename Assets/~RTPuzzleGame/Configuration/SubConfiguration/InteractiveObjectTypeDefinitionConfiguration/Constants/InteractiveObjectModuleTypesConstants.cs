using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RTPuzzle
{
    public class InteractiveObjectModuleTypesConstants 
    {
        public static List<Type> InteractiveObjectModuleTypes = new List<Type>() {
            typeof(TargetZoneModuleDefinition),
            typeof(LevelCompletionTriggerModuleDefinition),
            typeof(InteractiveObjectCutsceneControllerModuleDefinition),
            typeof(ActionInteractableObjectModuleDefinition),typeof(NearPlayerGameOverTriggerModuleDefinition),typeof(LaunchProjectileModuleDefinition),typeof(AttractiveObjectModuleDefinition),typeof(ModelObjectModuleDefinition),typeof(DisarmObjectModuleDefinition),typeof(GrabObjectModuleDefinition),//${addNewEntry}
        };
    }

}
