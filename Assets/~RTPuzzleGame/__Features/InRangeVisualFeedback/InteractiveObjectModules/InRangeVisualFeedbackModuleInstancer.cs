using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public static class InRangeVisualFeedbackModuleInstancer
    {
        public static InteractiveObjectTypeDefinitionInherentData BuildInRangeVisualFeedbackFromRange(RangeTypeObjectDefinitionInherentData RangeTypeObjectDefinitionInherentData)
        {
            var InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
            {
                RangeDefinitionModules = new Dictionary<Type, ScriptableObject>() {
                    {typeof(InRangeVisualFeedbackModuleDefinition),
                        new InRangeVisualFeedbackModuleDefinition(){
                               RangeTypeObjectDefinitionIDPicker = false,
                               RangeTypeObjectDefinitionInherentData = RangeTypeObjectDefinitionInherentData
                        }
                    }
                },
                RangeDefinitionModulesActivation = new Dictionary<Type, bool>() {
                    {typeof(InRangeVisualFeedbackModuleDefinition), true }
                }
            };
            return InteractiveObjectTypeDefinitionInherentData;
        }
    }
}
