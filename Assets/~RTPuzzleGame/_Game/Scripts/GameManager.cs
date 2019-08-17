using CoreGame;
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
        private NPCAIManagerContainer NPCAIManagerContainer;
        private PlayerActionManager PlayerActionManager;
        private TimeFlowManager TimeFlowManager;
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        private InRangeEffectManager InRangeEffectManager;
        private CooldownFeedManager CooldownFeedManager;
        private TimeFlowPlayPauseManager TimeFlowPlayPauseManager;
        private NpcInteractionRingRendererManager NpcInteractionRingRendererManager;
        private GameOverManager GameOverManager;
        private DottedLineRendererManager DottedLineRendererManager;
        private ObjectRepelLineVisualFeedbackManager ObjectRepelLineVisualFeedbackManager;
        private ObstaclesListenerManager ObstaclesListenerManager;
        private SquareObstaclesManager SquareObstaclesManager;
        private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;
        private InteractiveObjectContainer InteractiveObjectContainer;
        private RangeTypeObjectContainer RangeTypeContainer;
        private CameraMovementManager CameraMovementManager;
        private CircleFillBarRendererManager CircleFillBarRendererManager;
        private PuzzleTutorialEventSender PuzzleTutorialEventSender;
        private BlockingCutscenePlayer BlockingCutscenePlayer;

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

            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            PlayerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            TimeFlowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            GroundEffectsManagerV2 = GameObject.FindObjectOfType<GroundEffectsManagerV2>();
            InRangeEffectManager = GameObject.FindObjectOfType<InRangeEffectManager>();
            RangeTypeContainer = GameObject.FindObjectOfType<RangeTypeObjectContainer>();
            InteractiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            CooldownFeedManager = GameObject.FindObjectOfType<CooldownFeedManager>();
            TimeFlowPlayPauseManager = GameObject.FindObjectOfType<TimeFlowPlayPauseManager>();
            NpcInteractionRingRendererManager = GameObject.FindObjectOfType<NpcInteractionRingRendererManager>();
            GameOverManager = GameObject.FindObjectOfType<GameOverManager>();
            DottedLineRendererManager = GameObject.FindObjectOfType<DottedLineRendererManager>();
            ObjectRepelLineVisualFeedbackManager = GameObject.FindObjectOfType<ObjectRepelLineVisualFeedbackManager>();
            ObstaclesListenerManager = GameObject.FindObjectOfType<ObstaclesListenerManager>();
            SquareObstaclesManager = GameObject.FindObjectOfType<SquareObstaclesManager>();
            ObstacleFrustumCalculationManager = GameObject.FindObjectOfType<ObstacleFrustumCalculationManager>();
            CameraMovementManager = GameObject.FindObjectOfType<CameraMovementManager>();
            CircleFillBarRendererManager = GameObject.FindObjectOfType<CircleFillBarRendererManager>();
            PuzzleTutorialEventSender = GameObject.FindObjectOfType<PuzzleTutorialEventSender>();
            BlockingCutscenePlayer = GameObject.FindObjectOfType<BlockingCutscenePlayer>();

            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var TimeFlowBarManager = GameObject.FindObjectOfType<TimeFlowBarManager>();
            var PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            var LevelManager = GameObject.FindObjectOfType<LevelManager>();

            //Initialisations
            GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().Init();

            CameraMovementManager.Init();
            ObstacleFrustumCalculationManager.Init();
            ObstaclesListenerManager.Init();
            SquareObstaclesManager.Init();

            GroundEffectsManagerV2.Init(LevelManager.GetCurrentLevel());
            GameObject.FindObjectOfType<RangeEventsManager>().Init();
            InteractiveObjectContainer.Init();
            PlayerManagerDataRetriever.Init();
            PlayerManager.Init(gameInputManager);
            TimeFlowBarManager.Init(puzzleConfigurationManager.LevelConfiguration()[LevelManager.GetCurrentLevel()].AvailableTimeAmount);
            TimeFlowManager.Init(PlayerManagerDataRetriever, PlayerManager, gameInputManager, puzzleConfigurationManager, TimeFlowBarManager, LevelManager);
            GameOverManager.Init();
            GameObject.FindObjectOfType<PlayerActionEventManager>().Init();
            PlayerActionManager.Init();
            GameObject.FindObjectOfType<LaunchProjectileEventManager>().Init();
            InRangeEffectManager.Init();
            CooldownFeedManager.Init();
            PuzzleEventsManager.Init();
            TimeFlowPlayPauseManager.Init();
            GameObject.FindObjectOfType<PlayerActionPuzzleEventsManager>().Init();
            GameObject.FindObjectOfType<NpcInteractionRingContainer>().Init();
            //TODO here
            NpcInteractionRingRendererManager.Init();
            GameObject.FindObjectOfType<AIPositionsManager>().Init();
            GameObject.FindObjectOfType<NPCAIManagerContainer>().Init();
            ObjectRepelLineVisualFeedbackManager.Init();
            GameObject.FindObjectOfType<LevelCompletionManager>().Init();
            DottedLineRendererManager.Init();
            CircleFillBarRendererManager.Init();
            PuzzleTutorialEventSender.Init();

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
                    PuzzleTutorialEventSender.Tick(d);
                    BlockingCutscenePlayer.Tick(d);

                    PlayerActionManager.Tick(d);
                    PlayerManager.Tick(d);

                    CameraMovementManager.Tick(d);

                    ObstaclesListenerManager.Tick(d);
                    SquareObstaclesManager.Tick(d);
                    ObstacleFrustumCalculationManager.Tick(d);

                    TimeFlowManager.Tick(d);
                    GameOverManager.Tick(d);
                    CooldownFeedManager.Tick(d);
                    TimeFlowPlayPauseManager.Tick(TimeFlowManager.IsAbleToFlowTime());

                    NPCAIManagerContainer.TickAlways(d, TimeFlowManager.GetTimeAttenuation());
                    InteractiveObjectContainer.TickAlways(d);

                    if (TimeFlowManager.IsAbleToFlowTime() && !BlockingCutscenePlayer.Playing)
                    {
                        NPCAIManagerContainer.EnableAgents();
                        NPCAIManagerContainer.TickWhenTimeFlows(d, TimeFlowManager.GetTimeAttenuation());
                        InteractiveObjectContainer.Tick(d, TimeFlowManager.GetTimeAttenuation());

                        PlayerActionManager.TickWhenTimeFlows(d, TimeFlowManager.GetTimeAttenuation());
                    }
                    else
                    {
                        NPCAIManagerContainer.DisableAgents();
                    }

                    RangeTypeContainer.Tick(d);
                    GroundEffectsManagerV2.Tick(d);
                    InRangeEffectManager.Tick(d);
                    ObjectRepelLineVisualFeedbackManager.Tick(d);
                    NpcInteractionRingRendererManager.Tick(d);
                    DottedLineRendererManager.Tick();
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
            PuzzleDebugModule = GameObject.FindObjectOfType<PuzzleDebugModule>();
            PuzzleDebugModule.Init();
        }

        public void Tick(float d)
        {
            PuzzleDebugModule.Tick();
        }
    }
#endif

}