using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public static class VisualFeedbackEmitterModuleInstancer
    {
        public static InteractiveObjectTypeDefinitionInherentData BuildVisualFeedbackEmitterFromRange(RangeTypeObjectDefinitionInherentData RangeTypeObjectDefinitionInherentData)
        {
            var InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
            {
                RangeDefinitionModules = new Dictionary<Type, ScriptableObject>() {
                    {typeof(VisualFeedbackEmitterModuleDefinition),
                        new VisualFeedbackEmitterModuleDefinition(){
                               RangeTypeObjectDefinitionIDPicker = false,
                               RangeTypeObjectDefinitionInherentData = RangeTypeObjectDefinitionInherentData
                        }
                    }
                },
                RangeDefinitionModulesActivation = new Dictionary<Type, bool>() {
                    {typeof(VisualFeedbackEmitterModuleDefinition), true }
                }
            };
            return InteractiveObjectTypeDefinitionInherentData;
        }
    }
}
