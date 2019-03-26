using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleGameConfiguration", menuName = "Configuration/PuzzleGame/PuzzleGameConfiguration", order = 1)]
    public class PuzzleGameConfiguration : ScriptableObject
    {
        public ProjectileConfiguration ProjectileConfiguration;
        public TargetZonesConfiguration TargetZonesConfiguration;
        public AttractiveObjectConfiguration AttractiveObjectConfiguration;
        public LevelConfiguration LevelConfiguration;
        public SelectionWheelNodeConfiguration SelectionWheelNodeConfiguration;
        public AIComponentsConfiguration AIComponentsConfiguration;
        public PlayerActionConfiguration PlayerActionConfiguration;
    }
}
