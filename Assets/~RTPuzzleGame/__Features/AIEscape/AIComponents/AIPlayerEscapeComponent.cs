using UnityEngine;
using System;

namespace RTPuzzle
{
    [System.Serializable]
    [ModuleMetadata("AI", "Escape from player.")]
    [CreateAssetMenu(fileName = "AIPlayerEscapeComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIPlayerEscapeComponent", order = 1)]
    public class AIPlayerEscapeComponent : AbstractAIComponent
    {
        [WireCircle(R = 1, G = 1, B = 0)]
        public float EscapeDistance;
        [WireCircle(R = 1, G = 1, B = 0)]
        public float PlayerDetectionRadius;
        [WireArc(R = 1, G = 1, B = 0, Radius = 4f)]
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
