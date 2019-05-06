using UnityEngine;
using UnityEditor;
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

    public abstract class AbstractPlayerEscapeManager : InterfaceAIManager
    {
        public abstract void BeforeManagersUpdate(float d, float timeAttenuationFactor);

        public abstract bool IsManagerEnabled();

        public abstract void OnPlayerEscapeStart(AIPlayerEscapeDestinationCalculationType AIPlayerEscapeDestinationCalculationType);

        public abstract void OnDestinationReached();

        public abstract Vector3? OnManagerTick(float d, float timeAttenuationFactor);

        public abstract void OnStateReset();
    }

    public enum AIPlayerEscapeDestinationCalculationType
    {
        FAREST = 0,
        WITH_COLLIDERS = 1
    }
}
