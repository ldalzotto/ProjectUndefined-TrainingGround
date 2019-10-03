using CoreGame;
using InteractiveObjectTest;
using System.Collections;
using UnityEngine;

namespace RTPuzzle
{

    public class GameManager : AsbtractCoreGameManager
    {

        #region Persistance Dependencies
        private AInventoryMenu InventoryMenu;
        #endregion

        private PlayerManager PlayerManager;
        private PlayerManagerDataRetriever PlayerManagerDataRetriever;
        private AIManagerContainer NPCAIManagerContainer;
        private PlayerActionManager PlayerActionManager;
        private TimeFlowManager TimeFlowManager;
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        private InRangeVisualFeedbackManager InRangeEffectManager;
        private CooldownFeedManager CooldownFeedManager;
        private TimeFlowPlayPauseManager TimeFlowPlayPauseManager;
        private FovInteractionRingRendererManager NpcInteractionRingRendererManager;
        private GameOverManager GameOverManager;
        private DottedLineRendererManager DottedLineRendererManager;
        private ObstaclesListenerManager ObstaclesListenerManager;
        private SquareObstaclesManager SquareObstaclesManager;
        private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;
        private InteractiveObjectContainer InteractiveObjectContainer;
        private CameraMovementManager CameraMovementManager;
        private CircleFillBarRendererManager CircleFillBarRendererManager;
        private PuzzleTutorialEventSender PuzzleTutorialEventSender;
        private BlockingCutscenePlayerManager BlockingCutscenePlayer;
        private TutorialManager TutorialManager;
        private InteractiveObjectSelectionManager InteractiveObjectSelectionManager;
        private PuzzleDiscussionManager PuzzleDiscussionManager;

#if UNITY_EDITOR
        private EditorOnlyManagers EditorOnlyManagers;
#endif

        private void Awake()
        {
            GameObject.FindObjectOfType<GameManagerPersistanceInstance>().Init();
            this.AfterGameManagerPersistanceInstanceInitialization();
            //Level chunk initialization
            base.OnAwake(LevelType.PUZZLE);
        }

        protected virtual void AfterGameManagerPersistanceInstanceInitialization() { }

        private void Start()
        {
            base.OnStart();

            Coroutiner.Instance.StartCoroutine(this.EndOfFixedUpdate());

            InventoryMenu = AInventoryMenu.FindCurrentInstance();
            InventoryMenu.gameObject.SetActive(false);

            PlayerManager = PuzzleGameSingletonInstances.PlayerManager;
            NPCAIManagerContainer = PuzzleGameSingletonInstances.AIManagerContainer;
            PlayerActionManager = PuzzleGameSingletonInstances.PlayerActionManager;
            PlayerManagerDataRetriever = PuzzleGameSingletonInstances.PlayerManagerDataRetriever;
            TimeFlowManager = PuzzleGameSingletonInstances.TimeFlowManager;
            GroundEffectsManagerV2 = PuzzleGameSingletonInstances.GroundEffectsManagerV2;
            InRangeEffectManager = PuzzleGameSingletonInstances.InRangeEffectManager;
            InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            CooldownFeedManager = PuzzleGameSingletonInstances.CooldownFeedManager;
            TimeFlowPlayPauseManager = PuzzleGameSingletonInstances.TimeFlowPlayPauseManager;
            NpcInteractionRingRendererManager = PuzzleGameSingletonInstances.NpcInteractionRingRendererManager;
            GameOverManager = PuzzleGameSingletonInstances.GameOverManager;
            DottedLineRendererManager = PuzzleGameSingletonInstances.DottedLineRendererManager;
            ObstaclesListenerManager = PuzzleGameSingletonInstances.ObstaclesListenerManager;
            SquareObstaclesManager = PuzzleGameSingletonInstances.SquareObstaclesManager;
            ObstacleFrustumCalculationManager = PuzzleGameSingletonInstances.ObstacleFrustumCalculationManager;
            CameraMovementManager = CoreGameSingletonInstances.CameraMovementManager;
            CircleFillBarRendererManager = CoreGameSingletonInstances.CircleFillBarRendererManager;
            PuzzleTutorialEventSender = PuzzleGameSingletonInstances.PuzzleTutorialEventSender;
            BlockingCutscenePlayer = PuzzleGameSingletonInstances.BlockingCutscenePlayer;
            TutorialManager = CoreGameSingletonInstances.TutorialManager;
            InteractiveObjectSelectionManager = PuzzleGameSingletonInstances.InteractiveObjectSelectionManager;
            PuzzleDiscussionManager = PuzzleGameSingletonInstances.PuzzleDiscussionManager;

            var gameInputManager = CoreGameSingletonInstances.GameInputManager;
            var puzzleConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            var TimeFlowBarManager = PuzzleGameSingletonInstances.TimeFlowBarManager;
            var PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            var LevelManager = CoreGameSingletonInstances.LevelManager;

            //Initialisations
            PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.Init();
            
            CameraMovementManager.Init();
            ObstacleFrustumCalculationManager.Init();
            ObstaclesListenerManager.Init();
            SquareObstaclesManager.Init();
            GroundEffectsManagerV2.Init(LevelManager.GetCurrentLevel());
            PuzzleGameSingletonInstances.RangeEventsManager.Init();
            InteractiveObjectContainer.Init();
            PlayerManagerDataRetriever.Init();
            PlayerManager.Init(gameInputManager);
            TimeFlowBarManager.Init(puzzleConfigurationManager.LevelConfiguration()[LevelManager.GetCurrentLevel()].AvailableTimeAmount);
            TimeFlowManager.Init(PlayerManagerDataRetriever, PlayerManager, gameInputManager, puzzleConfigurationManager, TimeFlowBarManager, LevelManager);
            GameOverManager.Init();
            PuzzleGameSingletonInstances.PlayerActionEventManager.Init();
            PlayerActionManager.Init();
            InRangeEffectManager.Init();
            CooldownFeedManager.Init();
            PuzzleEventsManager.Init();
            TimeFlowPlayPauseManager.Init();
            PuzzleGameSingletonInstances.NpcInteractionRingContainer.Init();
            //TODO here
            NpcInteractionRingRendererManager.Init();
            PuzzleGameSingletonInstances.AIPositionsManager.Init();
            PuzzleGameSingletonInstances.AIManagerContainer.Init();
            PuzzleGameSingletonInstances.LevelCompletionManager.Init();
            DottedLineRendererManager.Init();
            CircleFillBarRendererManager.Init();
            PuzzleTutorialEventSender.Init();
            TutorialManager.Init();
            InteractiveObjectSelectionManager.Init(CoreGameSingletonInstances.GameInputManager);
            PuzzleDiscussionManager.Init();

            RangeObjectV2Manager.Get().Init();
            InteractiveObjectV2Manager.Get().Init();

#if UNITY_EDITOR
            EditorOnlyManagers = new EditorOnlyManagers();
            EditorOnlyManagers.Init();
#endif
        }

        private void Update()
        {
            if (!this.IsInitializing)
            {
                var d = Time.deltaTime;

                this.BeforeTick(d);

                if (!GameOverManager.OnGameOver)
                {
                    TutorialManager.Tick(d);

                    PuzzleTutorialEventSender.Tick(d);
                    BlockingCutscenePlayer.Tick(d);

                    PlayerActionManager.Tick(d);
                    PlayerManager.Tick(d);

                    CameraMovementManager.Tick(d);

                    RangeObjectV2Manager.Get().Tick(d);

                    ObstaclesListenerManager.Tick(d); //Position Change Check
                    SquareObstaclesManager.Tick(d); //Position Change Check
                    ObstacleFrustumCalculationManager.Tick(d); //Multi threaded calculation when needed

                    TimeFlowManager.Tick(d);
                    GameOverManager.Tick(d);
                    CooldownFeedManager.Tick(d);
                    TimeFlowPlayPauseManager.Tick(TimeFlowManager.IsAbleToFlowTime());

                    NPCAIManagerContainer.TickAlways(d, TimeFlowManager.GetTimeAttenuation());
                    InteractiveObjectContainer.TickAlways(d);

                    if (TimeFlowManager.IsAbleToFlowTime() && !BlockingCutscenePlayer.Playing)
                    {
                        InteractiveObjectContainer.TickBeforeAIUpdate(d, TimeFlowManager.GetTimeAttenuation());
                        NPCAIManagerContainer.EnableAgents();
                        NPCAIManagerContainer.TickWhenTimeFlows(d, TimeFlowManager.GetTimeAttenuation());
                        InteractiveObjectV2Manager.Get().Tick(d, TimeFlowManager.GetTimeAttenuation());
                        InteractiveObjectContainer.Tick(d, TimeFlowManager.GetTimeAttenuation());

                        PlayerActionManager.TickWhenTimeFlows(d, TimeFlowManager.GetTimeAttenuation());
                    }
                    else
                    {
                        NPCAIManagerContainer.DisableAgents();
                    }

                    InteractiveObjectV2Manager.Get().AfterTicks();

                    PuzzleDiscussionManager.Tick(d);
                    GroundEffectsManagerV2.Tick(d);
                    InRangeEffectManager.Tick(d);
                    NpcInteractionRingRendererManager.Tick(d);
                    DottedLineRendererManager.Tick();
                    InteractiveObjectSelectionManager.Tick(d);
                    CircleFillBarRendererManager.Tick(d);
                }

#if UNITY_EDITOR
                EditorOnlyManagers.Tick(d);
#endif
            }
        }

        private void LateUpdate()
        {
            var d = Time.deltaTime;
            if (!GameOverManager.OnGameOver)
            {
                PlayerManager.LateTick(d);
                PlayerActionManager.LateTick(d);
            }
        }

        private void FixedUpdate()
        {
            var d = Time.fixedDeltaTime;
            if (!GameOverManager.OnGameOver)
            {
                PlayerManager.FixedTick(d);
            }
        }

        private IEnumerator EndOfFixedUpdate()
        {
            yield return new WaitForFixedUpdate();

            if (!GameOverManager.OnGameOver)
            {
                NPCAIManagerContainer.EndOfFixedTick();
            }

            yield return this.EndOfFixedUpdate();
        }

        private void OnDestroy()
        {
            RangeObjectV2Manager.Get().OnDestroy();
            InteractiveObjectV2Manager.Get().OnDestroy();
        }

        private void OnDrawGizmos()
        {
            if (NPCAIManagerContainer != null)
            {
                NPCAIManagerContainer.GizmoTick();
            }

            if (PlayerActionManager != null)
            {
                PlayerActionManager.GizmoTick();
            }
        }

        private void OnGUI()
        {
            if (NPCAIManagerContainer != null)
            {
                NPCAIManagerContainer.GUITick();
            }

            if (PlayerActionManager != null)
            {
                PlayerActionManager.GUITick();
            }
        }

    }

#if UNITY_EDITOR
    public class EditorOnlyManagers
    {
        private PuzzleDebugModule PuzzleDebugModule;

        public void Init()
        {
            PuzzleDebugModule = PuzzleGameSingletonInstances.PuzzleDebugModule;
            PuzzleDebugModule.Init();
        }

        public void Tick(float d)
        {
            PuzzleDebugModule.Tick();
        }
    }
#endif

}