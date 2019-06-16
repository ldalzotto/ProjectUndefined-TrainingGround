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
        private LaunchProjectileContainerManager LaunchProjectileContainerManager;
        private CooldownFeedManager CooldownFeedManager;
        private TimeFlowPlayPauseManager TimeFlowPlayPauseManager;
        private AttractiveObjectsContainerManager AttractiveObjectsContainerManager;
        private RangeTypeContainer RangeTypeContainer;
        private NpcInteractionRingRendererManager NpcInteractionRingRendererManager;
        private GameOverManager GameOverManager;
        private ObjectRepelContainerManager ObjectRepelContainerManager;
        private DottedLineRendererManager DottedLineRendererManager;
        private ObjectRepelLineVisualFeedbackManager ObjectRepelLineVisualFeedbackManager;

#if UNITY_EDITOR
        private EditorOnlyManagers EditorOnlyManagers;
#endif

        private void Awake()
        {
            GameObject.FindObjectOfType<GameManagerPersistanceInstance>().Init();
            this.AfterGameManagerPersistanceInstanceInitialization();
            //Level chunk initialization
            base.OnAwake();
            var LevelManager = GameObject.FindObjectOfType<LevelManager>();
            LevelManager.Init(LevelType.PUZZLE);
        }

        protected virtual void AfterGameManagerPersistanceInstanceInitialization() { }

        private void Start()
        {
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
            LaunchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            CooldownFeedManager = GameObject.FindObjectOfType<CooldownFeedManager>();
            TimeFlowPlayPauseManager = GameObject.FindObjectOfType<TimeFlowPlayPauseManager>();
            AttractiveObjectsContainerManager = GameObject.FindObjectOfType<AttractiveObjectsContainerManager>();
            RangeTypeContainer = GameObject.FindObjectOfType<RangeTypeContainer>();
            NpcInteractionRingRendererManager = GameObject.FindObjectOfType<NpcInteractionRingRendererManager>();
            GameOverManager = GameObject.FindObjectOfType<GameOverManager>();
            ObjectRepelContainerManager = GameObject.FindObjectOfType<ObjectRepelContainerManager>();
            DottedLineRendererManager = GameObject.FindObjectOfType<DottedLineRendererManager>();
            ObjectRepelLineVisualFeedbackManager = GameObject.FindObjectOfType<ObjectRepelLineVisualFeedbackManager>();

            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var TimeFlowBarManager = GameObject.FindObjectOfType<TimeFlowBarManager>();
            var PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            var LevelManager = GameObject.FindObjectOfType<LevelManager>();

            //Initialisations
            GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().Init();
            GroundEffectsManagerV2.Init();
            GameObject.FindObjectOfType<RangeEventsManager>().Init();
            RangeTypeContainer.Init();
            GameObject.FindObjectOfType<TargetZoneContainer>().Init();
            PlayerManagerDataRetriever.Init();
            PlayerManager.Init(gameInputManager);
            TimeFlowBarManager.Init(puzzleConfigurationManager.LevelConfiguration()[LevelManager.GetCurrentLevel()].AvailableTimeAmount);
            TimeFlowManager.Init(PlayerManagerDataRetriever, PlayerManager, gameInputManager, puzzleConfigurationManager, TimeFlowBarManager, LevelManager);
            GameOverManager.Init();
            GameObject.FindObjectOfType<PlayerActionEventManager>().Init();
            PlayerActionManager.Init();
            LaunchProjectileContainerManager.Init();
            GameObject.FindObjectOfType<LaunchProjectileEventManager>().Init();
            InRangeEffectManager.Init();
            CooldownFeedManager.Init();
            PuzzleEventsManager.Init();
            TimeFlowPlayPauseManager.Init();
            GameObject.FindObjectOfType<PlayerActionPuzzleEventsManager>().Init();
            AttractiveObjectsContainerManager.Init();
            GameObject.FindObjectOfType<NpcInteractionRingContainer>().Init();
            NpcInteractionRingRendererManager.Init();
            GameObject.FindObjectOfType<NPCAIManagerContainer>().Init();
            GameObject.FindObjectOfType<ObjectRepelContainer>().Init();
            ObjectRepelLineVisualFeedbackManager.Init();
            ObjectRepelContainerManager.Init();
            AttractiveObjectsContainerManager.InitStaticInitials();
            GameObject.FindObjectOfType<LevelCompletionManager>().Init();
            DottedLineRendererManager.Init();

#if UNITY_EDITOR
            EditorOnlyManagers = new EditorOnlyManagers();
            EditorOnlyManagers.Init();
#endif
        }

        private void Update()
        {
            var d = Time.deltaTime;

            this.BeforeTick(d);

            if (!GameOverManager.OnGameOver)
            {
                PlayerActionManager.Tick(d);
                PlayerManager.Tick(d);
                TimeFlowManager.Tick(d);
                GameOverManager.Tick(d);
                CooldownFeedManager.Tick(d);
                TimeFlowPlayPauseManager.Tick(TimeFlowManager.IsAbleToFlowTime());

                NPCAIManagerContainer.TickAlways(d, TimeFlowManager.GetTimeAttenuation());

                if (TimeFlowManager.IsAbleToFlowTime())
                {
                    NPCAIManagerContainer.EnableAgents();
                    NPCAIManagerContainer.TickWhenTimeFlows(d, TimeFlowManager.GetTimeAttenuation());
                    LaunchProjectileContainerManager.Tick(d, TimeFlowManager.GetTimeAttenuation());
                    PlayerActionManager.TickWhenTimeFlows(d, TimeFlowManager.GetTimeAttenuation());
                    AttractiveObjectsContainerManager.Tick(d, TimeFlowManager.GetTimeAttenuation());
                    ObjectRepelContainerManager.Tick(d, TimeFlowManager.GetTimeAttenuation());
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
            }

#if UNITY_EDITOR
            EditorOnlyManagers.Tick(d);
#endif
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