using System;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [ModuleMetadata("AI", "Escape from target zone.")]
    [CreateAssetMenu(fileName = "AITargetZoneComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AITargetZoneComponent", order = 1)]
    public class AITargetZoneComponent : AbstractAIComponent
    {
        [WireCircle(B = 0, G = 1, R = 0)]
        public float TargetZoneEscapeDistance;

        public override InterfaceAIManager BuildManager()
        {
            return new AITargetZoneEscapeManager(this);
        }
    }

    public abstract class AbstractAITargetZoneManager : AbstractAIManager<AITargetZoneComponent>, InterfaceAIManager
    {
        #region State
        protected bool isEscapingFromTargetZone;
        #endregion
        protected AbstractAITargetZoneManager(AITargetZoneComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public abstract void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData);

        public abstract void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext);
        public abstract void TriggerTargetZoneEscape(ITargetZoneModuleDataRetriever ITargetZoneModuleDataRetriever);
        public abstract void OnDestinationReached();
        public abstract void OnStateReset();

        public virtual void BeforeManagersUpdate(float d, float timeAttenuationFactor) { }

        public bool IsManagerEnabled()
        {
            return this.isEscapingFromTargetZone;
        }

    }

}
