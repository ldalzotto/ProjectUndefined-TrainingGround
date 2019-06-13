using System;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AITargetZoneComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AITargetZoneComponent", order = 1)]
    public class AITargetZoneComponent : AbstractAIComponent
    {
        public float TargetZoneEscapeDistance;

        protected override Type abstractManagerType => typeof(AbstractAITargetZoneManager);
    }

    public abstract class AbstractAITargetZoneManager : InterfaceAIManager
    {
        #region State
        protected bool isEscapingFromTargetZone;
        #endregion

        public abstract Vector3? OnManagerTick(float d, float timeAttenuationFactor);
        public abstract void TriggerTargetZoneEscape(TargetZone targetZone);
        public abstract void OnDestinationReached();
        public abstract void OnStateReset();

        public virtual void BeforeManagersUpdate(float d, float timeAttenuationFactor) { }

        public bool IsManagerEnabled()
        {
            return this.isEscapingFromTargetZone;
        }

    }

}
