using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public static class VisualFeedbackEmitterModuleInstancer
    {
        public static InteractiveObjectTypeDefinitionInherentData BuildVisualFeedbackEmitterFromRange(RangeObjectInitialization RangeObjectInitialization)
        {
            var InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
            {
                RangeDefinitionModules = new Dictionary<Type, ScriptableObject>() {
                    {typeof(VisualFeedbackEmitterModuleDefinition),
                        new VisualFeedbackEmitterModuleDefinition(){
                            RangeObjectInitialization = RangeObjectInitialization
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
