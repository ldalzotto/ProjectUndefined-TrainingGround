using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public static class VisualFeedbackEmitterModuleInstancer
    {
        public static InteractiveObjectTypeDefinitionInherentData BuildVisualFeedbackEmitterFromRange(RangeObjectV2DefinitionInherentData RangeObjectV2DefinitionInherentData)
        {
            var InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
            {
                RangeDefinitionModules = new Dictionary<Type, ScriptableObject>() {
                    {typeof(VisualFeedbackEmitterModuleDefinition),
                        new VisualFeedbackEmitterModuleDefinition(){
                               RangeTypeObjectDefinitionInherentData = RangeObjectV2DefinitionInherentData
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
