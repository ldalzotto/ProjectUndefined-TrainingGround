using UnityEngine;
using System.Collections;
using System;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIEscapeWithoutTriggerComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIEscapeWithoutTriggerComponent", order = 1)]
    public class AIEscapeWithoutTriggerComponent : AbstractAIComponent
    {
        protected override Type abstractManagerType => typeof(AbstractAIEscapeWithoutTriggerManager);
    }

    public abstract class AbstractAIEscapeWithoutTriggerManager : MonoBehaviour, InterfaceAIManager
    {
        public abstract void BeforeManagersUpdate(float d, float timeAttenuationFactor);

        public abstract bool IsManagerEnabled();

        public abstract void OnDestinationReached();

        public abstract Vector3? OnManagerTick(float d, float timeAttenuationFactor);

        public abstract void OnStateReset();

        public abstract void OnEscapeStart(EscapeWithoutTriggerStartAIBehaviorEvent escapeWithoutTriggerStartAIBehaviorEvent);
    }

}
