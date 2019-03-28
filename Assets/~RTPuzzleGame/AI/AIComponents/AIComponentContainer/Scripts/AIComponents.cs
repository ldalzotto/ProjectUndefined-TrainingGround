using UnityEngine;
using UnityEditor;
using OdinSerializer;

namespace RTPuzzle
{

    [System.Serializable]
    public abstract class AbstractAIComponents : SerializedScriptableObject { }

    [System.Serializable]
    [CreateAssetMenu(fileName = "AIComponents", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIComponents", order = 1)]
    public class AIComponents : AbstractAIComponents
    {
        public AIPatrolComponent AIRandomPatrolComponent;
        public AIProjectileEscapeComponent AIProjectileEscapeComponent;
        public AITargetZoneComponent AITargetZoneComponent;
        public AIAttractiveObjectComponent AIAttractiveObjectComponent;
        public AIFearStunComponent AIFearStunComponent;
    }
}
