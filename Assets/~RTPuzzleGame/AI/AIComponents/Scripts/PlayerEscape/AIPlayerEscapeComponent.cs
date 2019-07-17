using UnityEngine;
using System;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIPlayerEscapeComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIPlayerEscapeComponent", order = 1)]
    public class AIPlayerEscapeComponent : AbstractAIComponent
    {
        public float EscapeDistance;
        public float PlayerDetectionRadius;
        public float EscapeSemiAngle;

        protected override Type abstractManagerType => typeof(AbstractPlayerEscapeManager);
    }

    public abstract class AbstractPlayerEscapeManager : MonoBehaviour, InterfaceAIManager
    {
        public abstract void BeforeManagersUpdate(float d, float timeAttenuationFactor);

        public abstract bool IsManagerEnabled();

        public abstract void OnPlayerEscapeStart();

        public abstract void OnDestinationReached();

        public abstract Vector3? OnManagerTick(float d, float timeAttenuationFactor);

        public abstract void OnStateReset();
    }

}
