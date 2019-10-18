using System.Collections;
using CoreGame;
using InteractiveObjects;
using Obstacle;
using RangeObjects;
using UnityEngine;

namespace RTPuzzle
{
    public class GameManager : AsbtractCoreGameManager
    {
        private BlockingCutscenePlayerManager BlockingCutscenePlayer;

#if UNITY_EDITOR
        private EditorOnlyManagers EditorOnlyManagers;
#endif

        #region Persistance Dependencies

        private AInventoryMenu InventoryMenu;

        #endregion

        private PuzzleDiscussionManager PuzzleDiscussionManager;
        private TutorialManager TutorialManager;

        private void Awake()
        {
            FindObjectOfType<GameManagerPersistanceInstance>().Init();
            AfterGameManagerPersistanceInstanceInitialization();
            //Level chunk initialization
            OnAwake(LevelType.PUZZLE);
        }

        protected virtual void AfterGameManagerPersistanceInstanceInitialization()
        {
        }

        private void Start()
        {
            OnStart();

            Coroutiner.Instance.StartCoroutine(EndOfFixedUpdate());

            InventoryMenu = AInventoryMenu.FindCurrentInstance();
            InventoryMenu.gameObject.SetActive(false);

            BlockingCutscenePlayer = PuzzleGameSingletonInstances.BlockingCutscenePlayer;
            TutorialManager = CoreGameSingletonInstances.TutorialManager;
            PuzzleDiscussionManager = PuzzleGameSingletonInstances.PuzzleDiscussionManager;

            var gameInputManager = CoreGameSingletonInstances.GameInputManager;
            var puzzleConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            var LevelManager = CoreGameSingletonInstances.LevelManager;


            RangeObjectV2Manager.Get().Init();
            GroundEffectsManagerV2.Get().Init(LevelManager.GetCurrentLevel());
            InteractiveObjectV2Manager.Get().Init();

            CameraMovementManager.Get().Init();

            TimeFlowBarManager.Get().Init(puzzleConfigurationManager.LevelConfiguration()[LevelManager.GetCurrentLevel()].AvailableTimeAmount);
            TimeFlowManager.Get().Init(TimeFlowBarManager.Get());
            TimeFlowPlayPauseManager.Get().Init();
            CircleFillBarRendererManager.Get().Init();
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
            if (!IsInitializing)
            {
                var d = Time.deltaTime;

                BeforeTick(d);

                if (!GameOverManager.Get().OnGameOver)
                {
                    TutorialManager.Tick(d);

                    PuzzleTutorialEventSenderManager.Get().Tick(d);
                    BlockingCutscenePlayer.Tick(d);

                    PlayerActionManager.Get().Tick(d);
                    PlayerInteractiveObjectManager.Get().TickAlways(d);

                    CameraMovementManager.Get().Tick(d);

                    ObstacleOcclusionCalculationManagerV2.Get().Tick(d);
                    RangeIntersectionCalculationManagerV2.Get().Tick(d);

                    RangeObjectV2Manager.Get().Tick(d);

                    TimeFlowManager.Get().Tick(d);
                    GameOverManager.Get().Tick(d);
                    TimeFlowPlayPauseManager.Get().Tick(TimeFlowManager.Get().IsAbleToFlowTime());

                    InteractiveObjectV2Manager.Get().TickAlways(d);

                    if (TimeFlowManager.Get().IsAbleToFlowTime() && !BlockingCutscenePlayer.Playing)
                    {
                        InteractiveObjectV2Manager.Get().Tick(d, TimeFlowManager.Get().GetTimeAttenuation());
                        PlayerActionManager.Get().TickWhenTimeFlows(d, TimeFlowManager.Get().GetTimeAttenuation());
                    }
                    else
                    {
                        InteractiveObjectV2Manager.Get().TickWhenTimeIsStopped();
                    }

                    InteractiveObjectV2Manager.Get().AfterTicks();

                    PuzzleDiscussionManager.Tick(d);
                    GroundEffectsManagerV2.Get().Tick(d);
                    DottedLineRendererManager.Get().Tick();
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
            if (!GameOverManager.Get().OnGameOver)
            {
                PlayerInteractiveObjectManager.Get().LateTick(d);
                InteractiveObjectV2Manager.Get().LateTick(d);
                PlayerActionManager.Get().LateTick(d);
            }

            ObstacleOcclusionCalculationManagerV2.Get().LateTick();
            RangeIntersectionCalculationManagerV2.Get().LateTick();
        }

        private void FixedUpdate()
        {
            var d = Time.fixedDeltaTime;
            if (!GameOverManager.Get().OnGameOver)
            {
                PlayerInteractiveObjectManager.Get().FixedTick(d);
                InteractiveObjectV2Manager.Get().FixedTick(d);
            }
        }

        private IEnumerator EndOfFixedUpdate()
        {
            yield return new WaitForFixedUpdate();

            if (!GameOverManager.Get().OnGameOver)
            {
            }

            yield return EndOfFixedUpdate();
        }

        private void OnDestroy()
        {
            GameSingletonManagers.Get().OnDestroy();
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                PlayerActionManager.Get().GizmoTick();
                ObstaclesListenerManager.Get().GizmoTick();
                RangeIntersectionCalculatorManager.Get().GizmoTick();
            }
        }

        private void OnGUI()
        {
            if (Application.isPlaying) PlayerActionManager.Get().GUITick();
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