﻿using CoreGame;

namespace RTPuzzle
{
    public static class PuzzleGameSingletonInstances
    {
        private static PuzzleEventsManager puzzleEventsManager;
        private static PuzzleGameConfigurationManager puzzleGameConfigurationManager;
        private static BlockingCutscenePlayerManager blockingCutscenePlayer;
        private static PuzzleStaticConfigurationContainer puzzleStaticConfigurationContainer;
        private static CooldownFeedManager cooldownFeedManager;
        private static PuzzleTutorialEventSender puzzleTutorialEventSender;
        private static PlayerActionEventManager playerActionEventManager;
        private static PuzzleDebugModule puzzleDebugModule;
        private static SelectionWheel puzzleSelectionWheel;
        private static PuzzleDiscussionWindowsContainer puzzleDiscussionWindowsContainer;
        private static PuzzleDiscussionManager puzzleDiscussionManager;

        public static PuzzleEventsManager PuzzleEventsManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleEventsManager, obj => puzzleEventsManager = obj); }
        public static PuzzleGameConfigurationManager PuzzleGameConfigurationManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleGameConfigurationManager, obj => puzzleGameConfigurationManager = obj); }
        public static BlockingCutscenePlayerManager BlockingCutscenePlayer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(blockingCutscenePlayer, obj => blockingCutscenePlayer = obj); }
        public static PuzzleStaticConfigurationContainer PuzzleStaticConfigurationContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleStaticConfigurationContainer, obj => puzzleStaticConfigurationContainer = obj); }
        public static CooldownFeedManager CooldownFeedManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(cooldownFeedManager, obj => cooldownFeedManager = obj); }
        public static PuzzleTutorialEventSender PuzzleTutorialEventSender { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleTutorialEventSender, obj => puzzleTutorialEventSender = obj); }
        public static PlayerActionEventManager PlayerActionEventManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(playerActionEventManager, obj => playerActionEventManager = obj); }
        public static PuzzleDebugModule PuzzleDebugModule { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleDebugModule, obj => puzzleDebugModule = obj); }
        public static SelectionWheel PuzzleSelectionWheel { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleSelectionWheel, obj => puzzleSelectionWheel = obj); }
        public static PuzzleDiscussionWindowsContainer PuzzleDiscussionWindowsContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleDiscussionWindowsContainer, obj => puzzleDiscussionWindowsContainer = obj); }
        public static PuzzleDiscussionManager PuzzleDiscussionManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleDiscussionManager, obj => puzzleDiscussionManager = obj); }
    }
}