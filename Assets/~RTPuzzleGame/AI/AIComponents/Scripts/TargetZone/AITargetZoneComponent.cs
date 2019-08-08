using System;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AITargetZoneComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AITargetZoneComponent", order = 1)]
    public class AITargetZoneComponent : AbstractAIComponent
    {
        public float TargetZoneEscapeDistance;
    }

    public abstract class AbstractAITargetZoneManager : AbstractAIManager, InterfaceAIManager
    {
        #region State
        protected bool isEscapingFromTargetZone;
        #endregion

        public abstract void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext);
        public abstract void TriggerTargetZoneEscape(TargetZoneModule targetZone);
        public abstract void OnDestinationReached();
        public abstract void OnStateReset();

        public virtual void BeforeManagersUpdate(float d, float timeAttenuationFactor) { }

        public bool IsManagerEnabled()
        {
            return this.isEscapingFromTargetZone;
        }

    }

}
