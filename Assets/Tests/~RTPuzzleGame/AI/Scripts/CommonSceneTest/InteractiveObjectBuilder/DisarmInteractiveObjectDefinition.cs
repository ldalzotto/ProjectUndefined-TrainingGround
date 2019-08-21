using UnityEngine;
using System.Collections;
using RTPuzzle;
using System.Collections.Generic;
using System;

namespace Tests
{

    public static class DisarmInteractiveObjectDefinition
    {
        public static InteractiveObjectInitialization OnlyDisarmObject(
                            InteractiveObjectTestID interactiveObjectTestID,
                            float disarmInteractionRange, float disarmTime)
        {
            var InteractiveObjectInitialization =
                new InteractiveObjectInitialization()
                {
                    InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                    {
                        RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                       {
                        {typeof(DisarmObjectModuleDefinition), new DisarmObjectModuleDefinition(){ DisarmObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].DisarmObjectID } },
                        {typeof(ModelObjectModuleDefinition), new ModelObjectModuleDefinition() }
                       },
                        RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                       {
                        {typeof(DisarmObjectModuleDefinition), true},
                        {typeof(ModelObjectModuleDefinition), true }
                       }
                    },
                    InteractiveObjectInitializationObject = new InteractiveObjectInitializationObject()
                    {
                        DisarmObjectInherentData = new DisarmObjectInherentData()
                        {
                            DisarmInteractionRange = disarmInteractionRange,
                            DisarmTime = disarmTime
                        }
                    }
                };

            InteractiveObjectInitialization.InteractiveObjectTypeDefinitionInherentData.InteractiveObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].InteractiveObjectID;
            InteractiveObjectInitialization.InitializeTestConfigurations(interactiveObjectTestID);
            return InteractiveObjectInitialization;
        }
    }

}
