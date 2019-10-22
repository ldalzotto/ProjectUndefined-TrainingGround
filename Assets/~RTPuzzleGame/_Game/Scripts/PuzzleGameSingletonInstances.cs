using CoreGame;

namespace RTPuzzle
{
    public static class PuzzleGameSingletonInstances
    {
        private static PuzzleStaticConfigurationContainer puzzleStaticConfigurationContainer;
        private static PuzzleDebugModule puzzleDebugModule;

        public static PuzzleStaticConfigurationContainer PuzzleStaticConfigurationContainer => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleStaticConfigurationContainer, obj => puzzleStaticConfigurationContainer = obj);

        public static PuzzleDebugModule PuzzleDebugModule => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleDebugModule, obj => puzzleDebugModule = obj);
    }
}