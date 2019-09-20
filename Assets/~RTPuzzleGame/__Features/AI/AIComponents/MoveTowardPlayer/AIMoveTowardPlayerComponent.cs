using UnityEngine;
using static AIMovementDefinitions;

namespace RTPuzzle
{

    [System.Serializable]
    [ModuleMetadata("AI", "Moving towards player.")]
    [CreateAssetMenu(fileName = "AIMoveTowardPlayerComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIMoveTowardPlayerComponent", order = 1)]
    public class AIMoveTowardPlayerComponent : AbstractAIComponent
    {
        public AIMovementSpeedDefinition AISpeed;

        public override InterfaceAIManager BuildManager()
        {
            return new AIMoveTowardPlayerManager(this);
        }
    }

    public abstract class AbstractAIMoveTowardPlayerManager : AbstractAIManager<AIMoveTowardPlayerComponent>, InterfaceAIManager
    {
        protected AbstractAIMoveTowardPlayerManager(AIMoveTowardPlayerComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public abstract void BeforeManagersUpdate(float d, float timeAttenuationFactor);
        public abstract void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData);
        public abstract bool IsManagerEnabled();
        public abstract void OnDestinationReached();
        public abstract void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext);
        public abstract void OnStateReset();
        public abstract bool OnSightInRangeEnter(SightInRangeEnterAIBehaviorEvent sightInRangeEnterAIBehaviorEvent);
        public abstract void OnSightInRangeExit(SightInRangeExitAIBehaviorEvent sightInRangeExitAIBehaviorEvent);

#if UNITY_EDITOR
        #region Test data retrieval
        public abstract ColliderWithCollisionType GetCurrentTarget();
        #endregion
#endif
    }

}
