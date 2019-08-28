using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public static class ActionInteractableObjectDefinition
    {
        public static InteractiveObjectInitialization ActionInteractableObjectOnly(InteractiveObjectTestID interactiveObjectTestID,
            float interactionRange, PlayerActionInherentData associatedPlayerAction)
        {
            var InteractiveObjectInitialization =
                new InteractiveObjectInitialization()
                {
                    InteractiveObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].InteractiveObjectID,
                    InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                    {
                        InteractiveObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].InteractiveObjectID,
                        RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                    {
                        {typeof(ActionInteractableObjectModuleDefinition), new ActionInteractableObjectModuleDefinition(){ ActionInteractableObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].ActionInteractableObjectID } }
                    },
                        RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                    {
                        {typeof(ActionInteractableObjectModuleDefinition), true }
                    }
                    },
                    InteractiveObjectInitializationObject = new InteractiveObjectInitializationObject()
                    {
                        ActionInteractableObjectModuleInitializationData = new ActionInteractableObjectModuleInitializationData()
                        {
                            ActionInteractableObjectInherentData = new ActionInteractableObjectInherentData() {
                                InteractionRange = interactionRange,
                                PlayerActionId = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].PlayerActionId
                            },
                            AssociatedPlayerActionInherentData = associatedPlayerAction
                        }
                    }
                };
            InteractiveObjectInitialization.InitializeTestConfigurations(interactiveObjectTestID);
            return InteractiveObjectInitialization;
        }
    }
}
