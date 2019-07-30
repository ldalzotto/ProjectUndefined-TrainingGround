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
        public RangeTypeConfiguration RangeTypeConfiguration;
        public DottedLineConfiguration DottedLineConfiguration;
        public RepelableObjectsConfiguration RepelableObjectsConfiguration;
        public DisarmObjectConfiguration DisarmObjectConfiguration;
        public PuzzleCutsceneConfiguration PuzzleCutsceneConfiguration;
    }
}
