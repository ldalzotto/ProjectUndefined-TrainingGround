using System.Collections;
using UnityEngine;
using CoreGame;

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
        private AbstractLevelTransitionManager LevelTransitionManager;
        private GameOverManager GameOverManager;

        private void Awake()
        {
            GameObject.FindObjectOfType<GameManagerPersistanceInstance>().Init();

            //Level chunk initialization
            base.OnAwake();
            var LevelManager = GameObject.FindObjectOfType<LevelManager>();
            LevelManager.Init(LevelType.PUZZLE);
        }


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
            LevelTransitionManager = GameObject.FindObjectOfType<AbstractLevelTransitionManager>();
            GameOverManager = GameObject.FindObjectOfType<GameOverManager>();

            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var TimeFlowBarManager = GameObject.FindObjectOfType<TimeFlowBarManager>();
            var PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            var LevelManager = GameObject.FindObjectOfType<LevelManager>();

            //Initialisations
            LevelTransitionManager.Init();
            GameObject.FindObjectOfType<TargetZoneContainer>().Init();
            GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().Init();
            PlayerManagerDataRetriever.Init();
            PlayerManager.Init(gameInputManager);
            TimeFlowBarManager.Init(puzzleConfigurationManager.LevelConfiguration()[LevelManager.GetCurrentLevel()].AvailableTimeAmount);
            TimeFlowManager.Init(PlayerManagerDataRetriever, PlayerManager, gameInputManager, puzzleConfigurationManager, TimeFlowBarManager, LevelManager);
            GameOverManager.Init();
            GameObject.FindObjectOfType<PlayerActionEventManager>().Init();
            PlayerActionManager.Init();
            LaunchProjectileContainerManager.Init();
            GameObject.FindObjectOfType<LaunchProjectileEventManager>().Init();
            GroundEffectsManagerV2.Init();
            InRangeEffectManager.Init();
            CooldownFeedManager.Init();
            PuzzleEventsManager.Init();
            TimeFlowPlayPauseManager.Init();
            GameObject.FindObjectOfType<PlayerActionPuzzleEventsManager>().Init();
            AttractiveObjectsContainerManager.Init();
            GameObject.FindObjectOfType<NpcInteractionRingContainer>().Init();
            NpcInteractionRingRendererManager.Init();
            GameObject.FindObjectOfType<NPCAIManagerContainer>().Init();
            GameObject.FindObjectOfType<RangeEventsManager>().Init();
            RangeTypeContainer.Init();
            GameObject.FindObjectOfType<LevelCompletionManager>().Init();
        }

        private void Update()
        {
            var d = Time.deltaTime;

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
                }
                else
                {
                    NPCAIManagerContainer.DisableAgents();
                }

                RangeTypeContainer.Tick(d);
                GroundEffectsManagerV2.Tick(d);
                InRangeEffectManager.Tick(d);
                NpcInteractionRingRendererManager.Tick(d);
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

}