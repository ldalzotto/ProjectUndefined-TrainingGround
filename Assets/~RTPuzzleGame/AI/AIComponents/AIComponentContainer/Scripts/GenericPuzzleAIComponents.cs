using UnityEngine;
using UnityEditor;
using OdinSerializer;

namespace RTPuzzle
{

    [System.Serializable]
    public abstract class AbstractAIComponents : SerializedScriptableObject { }

    [System.Serializable]
    [CreateAssetMenu(fileName = "GenericPuzzleAIComponents", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/GenericPuzzleAIComponents", order = 1)]
    public class GenericPuzzleAIComponents : AbstractAIComponents
    {
        public AIPatrolComponent AIRandomPatrolComponent;
        public AIProjectileEscapeComponent AIProjectileEscapeWithCollisionComponent;
        public AIProjectileEscapeComponent AIProjectileEscapeWithoutCollisionComponent;
        public AITargetZoneComponent AITargetZoneComponent;
        public AIAttractiveObjectComponent AIAttractiveObjectComponent;
        public AIFearStunComponent AIFearStunComponent;
    }
}
