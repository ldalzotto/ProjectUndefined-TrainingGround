using UnityEngine;
using System;

namespace RTPuzzle
{
    [System.Serializable]
    [ModuleMetadata("AI", "Escape from player.")]
    [CreateAssetMenu(fileName = "AIPlayerEscapeComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIPlayerEscapeComponent", order = 1)]
    public class AIPlayerEscapeComponent : AbstractAIComponent
    {
        public float EscapeDistance;
        public float PlayerDetectionRadius;
        public float EscapeSemiAngle;

        public override InterfaceAIManager BuildManager()
        {
            return new AIPlayerEscapeManager(this);
        }
    }

    public abstract class AbstractPlayerEscapeManager : AbstractAIManager<AIPlayerEscapeComponent>, InterfaceAIManager
    {
        protected AbstractPlayerEscapeManager(AIPlayerEscapeComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public abstract void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData);

        public abstract void BeforeManagersUpdate(float d, float timeAttenuationFactor);

        public abstract bool IsManagerEnabled();

        public abstract void OnPlayerEscapeStart();

        public abstract void OnDestinationReached();

        public abstract void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext);

        public abstract void OnStateReset();
    }

}
