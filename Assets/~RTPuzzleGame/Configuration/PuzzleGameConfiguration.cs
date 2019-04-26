using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleGameConfiguration", menuName = "Configuration/PuzzleGame/PuzzleGameConfiguration", order = 1)]
    public class PuzzleGameConfiguration : GameConfiguration
    {
        public ProjectileConfiguration ProjectileConfiguration;
        public TargetZonesConfiguration TargetZonesConfiguration;
        public AttractiveObjectConfiguration AttractiveObjectConfiguration;
        public LevelConfiguration LevelConfiguration;
        public SelectionWheelNodeConfiguration SelectionWheelNodeConfiguration;
        public AIComponentsConfiguration AIComponentsConfiguration;
        public PlayerActionConfiguration PlayerActionConfiguration;
        public ContextMarkVisualFeedbackConfiguration ContextMarkVisualFeedbackConfiguration;
    }
}
