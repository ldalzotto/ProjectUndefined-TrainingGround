using CoreGame;

namespace RTPuzzle
{
    public static class PuzzleGameSingletonInstances
    {
        private static PlayerManager playerManager;
        private static PuzzleEventsManager puzzleEventsManager;
        private static TimeFlowManager timeFlowManager;
        private static PlayerManagerDataRetriever playerManagerDataRetriever;
        private static PuzzleGameConfigurationManager puzzleGameConfigurationManager;
        private static BlockingCutscenePlayer blockingCutscenePlayer;
        private static PuzzleStaticConfigurationContainer puzzleStaticConfigurationContainer;
        private static InteractiveObjectContainer interactiveObjectContainer;
        private static DottedLineRendererManager dottedLineRendererManager;
        private static GroundEffectsManagerV2 groundEffectsManagerV2;
        private static FovInteractionRingContainer npcInteractionRingContainer;
        private static InRangeEffectManager inRangeEffectManager;
        private static RangeEventsManager rangeEventsManager;
        private static AIFeedbackContainer aIFeedbackContainer;
        private static PlayerActionPuzzleEventsManager playerActionPuzzleEventsManager;
        private static ObjectRepelLineVisualFeedbackManager objectRepelLineVisualFeedbackManager;
        private static RangeTypeObjectContainer rangeTypeObjectContainer;
        private static PlayerActionManager playerActionManager;
        private static LaunchProjectileEventManager launchProjectileEventManager;
        private static ObstaclesListenerManager obstaclesListenerManager;
        private static ObstacleFrustumCalculationManager obstacleFrustumCalculationManager;
        private static SquareObstaclesManager squareObstaclesManager;
        private static AIManagerContainer aIManagerContainer;
        private static CooldownFeedManager cooldownFeedManager;
        private static TimeFlowPlayPauseManager timeFlowPlayPauseManager;
        private static FovInteractionRingRendererManager npcInteractionRingRendererManager;
        private static GameOverManager gameOverManager;
        private static PuzzleTutorialEventSender puzzleTutorialEventSender;
        private static InteractiveObjectSelectionManager interactiveObjectSelectionManager;
        private static TimeFlowBarManager timeFlowBarManager;
        private static PlayerActionEventManager playerActionEventManager;
        private static AIPositionsManager aIPositionsManager;
        private static LevelCompletionManager levelCompletionManager;
        private static PuzzleDebugModule puzzleDebugModule;
        private static SelectionWheel puzzleSelectionWheel;
        private static DottedLineContainer dottedLineContainer;

        public static PlayerManager PlayerManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(playerManager, obj => playerManager = obj); }
        public static PuzzleEventsManager PuzzleEventsManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleEventsManager, obj => puzzleEventsManager = obj); }
        public static TimeFlowManager TimeFlowManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(timeFlowManager, obj => timeFlowManager = obj); }
        public static PlayerManagerDataRetriever PlayerManagerDataRetriever { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(playerManagerDataRetriever, obj => playerManagerDataRetriever = obj); }
        public static PuzzleGameConfigurationManager PuzzleGameConfigurationManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleGameConfigurationManager, obj => puzzleGameConfigurationManager = obj); }
        public static BlockingCutscenePlayer BlockingCutscenePlayer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(blockingCutscenePlayer, obj => blockingCutscenePlayer = obj); }
        public static PuzzleStaticConfigurationContainer PuzzleStaticConfigurationContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleStaticConfigurationContainer, obj => puzzleStaticConfigurationContainer = obj); }
        public static InteractiveObjectContainer InteractiveObjectContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(interactiveObjectContainer, obj => interactiveObjectContainer = obj); }
        public static DottedLineRendererManager DottedLineRendererManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(dottedLineRendererManager, obj => dottedLineRendererManager = obj); }
        public static GroundEffectsManagerV2 GroundEffectsManagerV2 { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(groundEffectsManagerV2, obj => groundEffectsManagerV2 = obj); }
        public static FovInteractionRingContainer NpcInteractionRingContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(npcInteractionRingContainer, obj => npcInteractionRingContainer = obj); }
        public static InRangeEffectManager InRangeEffectManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(inRangeEffectManager, obj => inRangeEffectManager = obj); }
        public static RangeEventsManager RangeEventsManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(rangeEventsManager, obj => rangeEventsManager = obj); }
        public static AIFeedbackContainer AIFeedbackContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(aIFeedbackContainer, obj => aIFeedbackContainer = obj); }
        public static PlayerActionPuzzleEventsManager PlayerActionPuzzleEventsManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(playerActionPuzzleEventsManager, obj => playerActionPuzzleEventsManager = obj); }
        public static ObjectRepelLineVisualFeedbackManager ObjectRepelLineVisualFeedbackManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(objectRepelLineVisualFeedbackManager, obj => objectRepelLineVisualFeedbackManager = obj); }
        public static RangeTypeObjectContainer RangeTypeObjectContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(rangeTypeObjectContainer, obj => rangeTypeObjectContainer = obj); }
        public static PlayerActionManager PlayerActionManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(playerActionManager, obj => playerActionManager = obj); }
        public static LaunchProjectileEventManager LaunchProjectileEventManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(launchProjectileEventManager, obj => launchProjectileEventManager = obj); }
        public static ObstaclesListenerManager ObstaclesListenerManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(obstaclesListenerManager, obj => obstaclesListenerManager = obj); }
        public static ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(obstacleFrustumCalculationManager, obj => obstacleFrustumCalculationManager = obj); }
        public static SquareObstaclesManager SquareObstaclesManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(squareObstaclesManager, obj => squareObstaclesManager = obj); }
        public static AIManagerContainer AIManagerContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(aIManagerContainer, obj => aIManagerContainer = obj); }
        public static CooldownFeedManager CooldownFeedManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(cooldownFeedManager, obj => cooldownFeedManager = obj); }
        public static TimeFlowPlayPauseManager TimeFlowPlayPauseManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(timeFlowPlayPauseManager, obj => timeFlowPlayPauseManager = obj); }
        public static FovInteractionRingRendererManager NpcInteractionRingRendererManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(npcInteractionRingRendererManager, obj => npcInteractionRingRendererManager = obj); }
        public static GameOverManager GameOverManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(gameOverManager, obj => gameOverManager = obj); }
        public static PuzzleTutorialEventSender PuzzleTutorialEventSender { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleTutorialEventSender, obj => puzzleTutorialEventSender = obj); }
        public static InteractiveObjectSelectionManager InteractiveObjectSelectionManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(interactiveObjectSelectionManager, obj => interactiveObjectSelectionManager = obj); }
        public static TimeFlowBarManager TimeFlowBarManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(timeFlowBarManager, obj => timeFlowBarManager = obj); }
        public static PlayerActionEventManager PlayerActionEventManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(playerActionEventManager, obj => playerActionEventManager = obj); }
        public static AIPositionsManager AIPositionsManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(aIPositionsManager, obj => aIPositionsManager = obj); }
        public static LevelCompletionManager LevelCompletionManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(levelCompletionManager, obj => levelCompletionManager = obj); }
        public static PuzzleDebugModule PuzzleDebugModule { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleDebugModule, obj => puzzleDebugModule = obj); }
        public static SelectionWheel PuzzleSelectionWheel { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(puzzleSelectionWheel, obj => puzzleSelectionWheel = obj); }
        public static DottedLineContainer DottedLineContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(dottedLineContainer, obj => dottedLineContainer = obj); }
    }
}