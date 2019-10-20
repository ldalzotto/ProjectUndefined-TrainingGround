using CoreGame;

namespace RTPuzzle
{
    public static class PuzzleGameSingletonInstances
    {
        private static PuzzleGameConfigurationManager puzzleGameConfigurationManager;
        private static BlockingCutscenePlayerManager blockingCutscenePlayer;
        private static PuzzleStaticConfigurationContainer puzzleStaticConfigurationContainer;
        private static PuzzleDebugModule puzzleDebugModule;

        public static PuzzleGameConfigurationManager PuzzleGameConfigurationManager => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleGameConfigurationManager, obj => puzzleGameConfigurationManager = obj);

        public static BlockingCutscenePlayerManager BlockingCutscenePlayer => CoreGameSingletonInstances.FindAndSetInstanceIfNull(blockingCutscenePlayer, obj => blockingCutscenePlayer = obj);

        public static PuzzleStaticConfigurationContainer PuzzleStaticConfigurationContainer => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleStaticConfigurationContainer, obj => puzzleStaticConfigurationContainer = obj);

        public static PuzzleDebugModule PuzzleDebugModule => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleDebugModule, obj => puzzleDebugModule = obj);
    }
}