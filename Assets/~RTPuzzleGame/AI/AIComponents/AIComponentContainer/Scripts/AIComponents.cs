using UnityEngine;
using UnityEditor;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIComponents", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIComponents", order = 1)]
    public class AIComponents : ScriptableObject
    {
        public AIRandomPatrolComponent AIRandomPatrolComponent;
        public AIProjectileEscapeComponent AIProjectileEscapeComponent;
        public AITargetZoneComponent AITargetZoneComponent;
        public AIFearStunComponent AIFearStunComponent;

    }
}
