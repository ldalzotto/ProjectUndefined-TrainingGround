using UnityEngine;
using UnityEditor;
using OdinSerializer;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIComponents", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIComponents", order = 1)]
    public class AIComponents : SerializedScriptableObject
    {
        public AIPatrolComponent AIRandomPatrolComponent;
        public AIProjectileEscapeComponent AIProjectileEscapeComponent;
        public AITargetZoneComponent AITargetZoneComponent;
        public AIAttractiveObjectComponent AIAttractiveObjectComponent;
        public AIFearStunComponent AIFearStunComponent;

    }
}
