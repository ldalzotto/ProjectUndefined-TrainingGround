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

        private PlayerActionManager PlayerActionManager;
        private TimeFlowManager TimeFlowManager;
        private CooldownFeedManager CooldownFeedManager;
        private TimeFlowPlayPauseManager TimeFlowPlayPauseManager;
        private GameOverManager GameOverManager;
        private DottedLineRendererManager DottedLineRendererManager;
        private ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2;
        private CameraMovementManager CameraMovementManager;
        private PuzzleTutorialEventSender PuzzleTutorialEventSender;
        private BlockingCutscenePlayerManager BlockingCutscenePlayer;
        private TutorialManager TutorialManager;
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

            PlayerActionManager = PuzzleGameSingletonInstances.PlayerActionManager;
            TimeFlowManager = PuzzleGameSingletonInstances.TimeFlowManager;
            CooldownFeedManager = PuzzleGameSingletonInstances.CooldownFeedManager;
            TimeFlowPlayPauseManager = PuzzleGameSingletonInstances.TimeFlowPlayPauseManager;
            GameOverManager = PuzzleGameSingletonInstances.GameOverManager;
            DottedLineRendererManager = PuzzleGameSingletonInstances.DottedLineRendererManager;
            CameraMovementManager = CoreGameSingletonInstances.CameraMovementManager;
            PuzzleTutorialEventSender = PuzzleGameSingletonInstances.PuzzleTutorialEventSender;
            BlockingCutscenePlayer = PuzzleGameSingletonInstances.BlockingCutscenePlayer;
            TutorialManager = CoreGameSingletonInstances.TutorialManager;
            PuzzleDiscussionManager = PuzzleGameSingletonInstances.PuzzleDiscussionManager;

            var gameInputManager = CoreGameSingletonInstances.GameInputManager;
            var puzzleConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            var TimeFlowBarManager = PuzzleGameSingletonInstances.TimeFlowBarManager;
            var PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            var LevelManager = CoreGameSingletonInstances.LevelManager;

            CameraMovementManager.Init();
            GroundEffectsManagerV2.Get().Init(LevelManager.GetCurrentLevel());

            RangeObjectV2Manager.Get().Init();
            InteractiveObjectV2Manager.Get().Init();

            TimeFlowBarManager.Init(puzzleConfigurationManager.LevelConfiguration()[LevelManager.GetCurrentLevel()].AvailableTimeAmount);
            TimeFlowManager.Init(gameInputManager, puzzleConfigurationManager, TimeFlowBarManager, LevelManager);
            GameOverManager.Init();
            PuzzleGameSingletonInstances.PlayerActionEventManager.Init();
            PlayerActionManager.Init();
            CooldownFeedManager.Init();
            PuzzleEventsManager.Init();
            TimeFlowPlayPauseManager.Init();
            DottedLineRendererManager.Init();
            CircleFillBarRendererManager.Get().Init();
            PuzzleTutorialEventSender.Init();
            TutorialManager.Init();
            InteractiveObjectSelectionManager.Get().Init(CoreGameSingletonInstances.GameInputManager);
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

                    ObstacleOcclusionCalculationManagerV2.Get().Tick(d);
                    RangeIntersectionCalculationManagerV2.Get().Tick(d);

                    RangeObjectV2Manager.Get().Tick(d);

                    TimeFlowManager.Tick(d);
                    GameOverManager.Tick(d);
                    CooldownFeedManager.Tick(d);
                    TimeFlowPlayPauseManager.Tick(TimeFlowManager.IsAbleToFlowTime());

                    InteractiveObjectV2Manager.Get().TickAlways(d);

                    if (TimeFlowManager.IsAbleToFlowTime() && !BlockingCutscenePlayer.Playing)
                    {
                        InteractiveObjectV2Manager.Get().Tick(d, TimeFlowManager.GetTimeAttenuation());
                        PlayerActionManager.TickWhenTimeFlows(d, TimeFlowManager.GetTimeAttenuation());
                    }
                    else
                    {
                        InteractiveObjectV2Manager.Get().TickWhenTimeIsStopped();
                    }

                    InteractiveObjectV2Manager.Get().AfterTicks();

                    PuzzleDiscussionManager.Tick(d);
                    GroundEffectsManagerV2.Get().Tick(d);
                    DottedLineRendererManager.Tick();
                    InteractiveObjectSelectionManager.Get().Tick(d);
                    CircleFillBarRendererManager.Get().Tick(d);
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
            }

            yield return this.EndOfFixedUpdate();
        }

        private void OnDestroy()
        {
            GameSingletonManagers.Get().OnDestroy();
        }

        private void OnDrawGizmos()
        {
            if (PlayerActionManager != null)
            {
                PlayerActionManager.GizmoTick();
            }

            ObstaclesListenerManager.Get().GizmoTick();
            RangeIntersectionCalculatorV2Manager.Get().GizmoTick();
        }

        private void OnGUI()
        {

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