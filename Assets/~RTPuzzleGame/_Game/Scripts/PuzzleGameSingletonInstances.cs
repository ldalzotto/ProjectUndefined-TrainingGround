using CoreGame;

namespace RTPuzzle
{
    public static class PuzzleGameSingletonInstances
    {
        private static PuzzleEventsManager puzzleEventsManager;
        private static TimeFlowManager timeFlowManager;
        private static PuzzleGameConfigurationManager puzzleGameConfigurationManager;
        private static BlockingCutscenePlayerManager blockingCutscenePlayer;
        private static PuzzleStaticConfigurationContainer puzzleStaticConfigurationContainer;
        private static DottedLineRendererManager dottedLineRendererManager;
        private static RangeEventsManager rangeEventsManager;
        private static PlayerActionManager playerActionManager;
        private static CooldownFeedManager cooldownFeedManager;
        private static TimeFlowPlayPauseManager timeFlowPlayPauseManager;
        private static GameOverManager gameOverManager;
        private static PuzzleTutorialEventSender puzzleTutorialEventSender;
        private static TimeFlowBarManager timeFlowBarManager;
        private static PlayerActionEventManager playerActionEventManager;
        private static LevelCompletionManager levelCompletionManager;
        private static PuzzleDebugModule puzzleDebugModule;
        private static SelectionWheel puzzleSelectionWheel;
        private static DottedLineContainer dottedLineContainer;
        private static PuzzleDiscussionWindowsContainer puzzleDiscussionWindowsContainer;
        private static PuzzleDiscussionManager puzzleDiscussionManager;

        public static PuzzleEventsManager PuzzleEventsManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleEventsManager, obj => puzzleEventsManager = obj); }
        public static TimeFlowManager TimeFlowManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(timeFlowManager, obj => timeFlowManager = obj); }
        public static PuzzleGameConfigurationManager PuzzleGameConfigurationManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleGameConfigurationManager, obj => puzzleGameConfigurationManager = obj); }
        public static BlockingCutscenePlayerManager BlockingCutscenePlayer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(blockingCutscenePlayer, obj => blockingCutscenePlayer = obj); }
        public static PuzzleStaticConfigurationContainer PuzzleStaticConfigurationContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleStaticConfigurationContainer, obj => puzzleStaticConfigurationContainer = obj); }
        public static DottedLineRendererManager DottedLineRendererManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(dottedLineRendererManager, obj => dottedLineRendererManager = obj); }
        public static RangeEventsManager RangeEventsManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(rangeEventsManager, obj => rangeEventsManager = obj); }
        public static PlayerActionManager PlayerActionManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(playerActionManager, obj => playerActionManager = obj); }
        public static CooldownFeedManager CooldownFeedManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(cooldownFeedManager, obj => cooldownFeedManager = obj); }
        public static TimeFlowPlayPauseManager TimeFlowPlayPauseManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(timeFlowPlayPauseManager, obj => timeFlowPlayPauseManager = obj); }
        public static GameOverManager GameOverManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(gameOverManager, obj => gameOverManager = obj); }
        public static PuzzleTutorialEventSender PuzzleTutorialEventSender { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleTutorialEventSender, obj => puzzleTutorialEventSender = obj); }
        public static TimeFlowBarManager TimeFlowBarManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(timeFlowBarManager, obj => timeFlowBarManager = obj); }
        public static PlayerActionEventManager PlayerActionEventManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(playerActionEventManager, obj => playerActionEventManager = obj); }
        public static LevelCompletionManager LevelCompletionManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(levelCompletionManager, obj => levelCompletionManager = obj); }
        public static PuzzleDebugModule PuzzleDebugModule { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleDebugModule, obj => puzzleDebugModule = obj); }
        public static SelectionWheel PuzzleSelectionWheel { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleSelectionWheel, obj => puzzleSelectionWheel = obj); }
        public static DottedLineContainer DottedLineContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(dottedLineContainer, obj => dottedLineContainer = obj); }
        public static PuzzleDiscussionWindowsContainer PuzzleDiscussionWindowsContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleDiscussionWindowsContainer, obj => puzzleDiscussionWindowsContainer = obj); }
        public static PuzzleDiscussionManager PuzzleDiscussionManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleDiscussionManager, obj => puzzleDiscussionManager = obj); }
    }
}