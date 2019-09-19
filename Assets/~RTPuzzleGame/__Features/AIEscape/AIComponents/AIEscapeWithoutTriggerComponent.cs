using UnityEngine;
using System.Collections;
using System;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIEscapeWithoutTriggerComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIEscapeWithoutTriggerComponent", order = 1)]
    public class AIEscapeWithoutTriggerComponent : AbstractAIComponent
    {
        public override InterfaceAIManager BuildManager()
        {
            return new AIEscapeWithoutTriggerManager(this);
        }
    }

    public abstract class AbstractAIEscapeWithoutTriggerManager : AbstractAIManager<AIEscapeWithoutTriggerComponent>, InterfaceAIManager
    {
        protected AbstractAIEscapeWithoutTriggerManager(AIEscapeWithoutTriggerComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public abstract void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData);

        public abstract void BeforeManagersUpdate(float d, float timeAttenuationFactor);

        public abstract bool IsManagerEnabled();

        public abstract void OnDestinationReached();

        public abstract void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext);

        public abstract void OnStateReset();

        public abstract void OnEscapeStart(EscapeWithoutTriggerStartAIBehaviorEvent escapeWithoutTriggerStartAIBehaviorEvent);
    }

}
