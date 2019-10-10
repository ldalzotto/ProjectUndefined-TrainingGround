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
        private ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2;
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
            
            NPCAIManagerContainer = PuzzleGameSingletonInstances.AIManagerContainer;
            PlayerActionManager = PuzzleGameSingletonInstances.PlayerActionManager;
            TimeFlowManager = PuzzleGameSingletonInstances.TimeFlowManager;
            GroundEffectsManagerV2 = PuzzleGameSingletonInstances.GroundEffectsManagerV2;
            InRangeEffectManager = PuzzleGameSingletonInstances.InRangeEffectManager;
            InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            CooldownFeedManager = PuzzleGameSingletonInstances.CooldownFeedManager;
            TimeFlowPlayPauseManager = PuzzleGameSingletonInstances.TimeFlowPlayPauseManager;
            NpcInteractionRingRendererManager = PuzzleGameSingletonInstances.NpcInteractionRingRendererManager;
            GameOverManager = PuzzleGameSingletonInstances.GameOverManager;
            DottedLineRendererManager = PuzzleGameSingletonInstances.DottedLineRendererManager;
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
            GroundEffectsManagerV2.Init(LevelManager.GetCurrentLevel());
            PuzzleGameSingletonInstances.RangeEventsManager.Init();
            
            RangeObjectV2Manager.Get().Init();
            InteractiveObjectV2Manager.Get().Init();

            InteractiveObjectContainer.Init();
            TimeFlowBarManager.Init(puzzleConfigurationManager.LevelConfiguration()[LevelManager.GetCurrentLevel()].AvailableTimeAmount);
            TimeFlowManager.Init(gameInputManager, puzzleConfigurationManager, TimeFlowBarManager, LevelManager);
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
                    PlayerInteractiveObjectManager.Get().TickAlways(d);

                    CameraMovementManager.Tick(d);

                    RangeObjectV2Manager.Get().Tick(d);

                    ObstacleOcclusionCalculationManagerV2.Get().Tick(d);
                    RangeIntersectionCalculationManagerV2.Get().Tick(d);

                    TimeFlowManager.Tick(d);
                    GameOverManager.Tick(d);
                    CooldownFeedManager.Tick(d);
                    TimeFlowPlayPauseManager.Tick(TimeFlowManager.IsAbleToFlowTime());

                    NPCAIManagerContainer.TickAlways(d, TimeFlowManager.GetTimeAttenuation());
                    InteractiveObjectV2Manager.Get().TickAlways(d);
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
                        InteractiveObjectV2Manager.Get().TickWhenTimeIsStopped();
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
                PlayerInteractiveObjectManager.Get().LateTick(d);
                InteractiveObjectV2Manager.Get().LateTick(d);
                PlayerActionManager.LateTick(d);
            }
            
            ObstacleOcclusionCalculationManagerV2.Get().LateTick();
            RangeIntersectionCalculationManagerV2.Get().LateTick();
        }

        private void FixedUpdate()
        {
            var d = Time.fixedDeltaTime;
            if (!GameOverManager.OnGameOver)
            {
                PlayerInteractiveObjectManager.Get().FixedTick(d);
                InteractiveObjectV2Manager.Get().FixedTick(d);
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
            ObstacleOcclusionCalculationManagerV2.Get().OnDestroy();
            SquareObstacleSystemManager.Get().OnDestroy();
            ObstaclesListenerManager.Get().OnDestroy();
            RangeIntersectionCalculationManagerV2.Get().OnDestroy();
            PlayerInteractiveObjectManager.Get().OnDestroy();
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

            ObstaclesListenerManager.Get().GizmoTick();
            RangeIntersectionCalculatorV2Manager.Get().GizmoTick();
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