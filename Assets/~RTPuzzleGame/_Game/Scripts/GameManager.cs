using UnityEngine;

namespace RTPuzzle
{

    public class GameManager : MonoBehaviour
    {

        public LevelZonesID PuzzleId;

        #region Persistance Dependencies
        private AInventoryMenu InventoryMenu;
        #endregion

        private PlayerManager PlayerManager;
        private PlayerManagerDataRetriever PlayerManagerDataRetriever;
        //  private NPCAIManager NpcAiManager;
        private NPCAIManagerContainer NPCAIManagerContainer;
        private PlayerActionManager PlayerActionManager;
        private TimeFlowManager TimeFlowManager;
        private GroundEffectsManager GroundEffectsManager;
        private LaunchProjectileContainerManager LaunchProjectileContainerManager;
        private CooldownFeedManager CooldownFeedManager;
        private TimeFlowPlayPauseManager TimeFlowPlayPauseManager;
        private AttractiveObjectsContainerManager AttractiveObjectsContainerManager;

        private void Start()
        {
            InventoryMenu = AInventoryMenu.FindCurrentInstance();
            InventoryMenu.gameObject.SetActive(false);

            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            //  NpcAiManager = GameObject.FindObjectOfType<NPCAIManager>();
            NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            PlayerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            TimeFlowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            GroundEffectsManager = GameObject.FindObjectOfType<GroundEffectsManager>();
            LaunchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            CooldownFeedManager = GameObject.FindObjectOfType<CooldownFeedManager>();
            TimeFlowPlayPauseManager = GameObject.FindObjectOfType<TimeFlowPlayPauseManager>();
            AttractiveObjectsContainerManager = GameObject.FindObjectOfType<AttractiveObjectsContainerManager>();

            //Initialisations
            GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().Init();
            GameObject.FindObjectOfType<AIComponentsManager>().Init();
            PlayerManagerDataRetriever.Init();
            PlayerManager.Init();
            // NpcAiManager.Init();
            TimeFlowManager.Init(PuzzleId);
            GameObject.FindObjectOfType<PlayerActionEventManager>().Init();
            PlayerActionManager.Init(PuzzleId);
            LaunchProjectileContainerManager.Init();
            GameObject.FindObjectOfType<LaunchProjectileEventManager>().Init();
            GameObject.FindObjectOfType<GroundCollision>().Init();
            GroundEffectsManager.Init();
            CooldownFeedManager.Init();
            GameObject.FindObjectOfType<PuzzleEventsManager>().Init();
            TimeFlowPlayPauseManager.Init();
            GameObject.FindObjectOfType<PlayerActionPuzzleEventsManager>().Init();
            GameObject.FindObjectOfType<LevelManager>().Init(PuzzleId);
            AttractiveObjectsContainerManager.Init();
            GameObject.FindObjectOfType<NPCAIManagerContainer>().Init();
        }

        private void Update()
        {
            var d = Time.deltaTime;
            PlayerActionManager.Tick(d);
            PlayerManager.Tick(d);
            TimeFlowManager.Tick(d);
            GroundEffectsManager.Tick(d);
            CooldownFeedManager.Tick(d);
            TimeFlowPlayPauseManager.Tick(TimeFlowManager.IsAbleToFlowTime());

            NPCAIManagerContainer.TickAlways(d, TimeFlowManager.GetTimeAttenuation());
            //NpcAiManager.TickAlways(d, TimeFlowManager.GetTimeAttenuation());

            if (TimeFlowManager.IsAbleToFlowTime())
            {
                NPCAIManagerContainer.EnableAgents();
                NPCAIManagerContainer.TickWhenTimeFlows(d, TimeFlowManager.GetTimeAttenuation());
                //  NpcAiManager.EnableAgent();
                //   NpcAiManager.TickWhenTimeFlows(d, TimeFlowManager.GetTimeAttenuation());
                LaunchProjectileContainerManager.Tick(d, TimeFlowManager.GetTimeAttenuation());
                PlayerActionManager.TickWhenTimeFlows(d, TimeFlowManager.GetTimeAttenuation());
                AttractiveObjectsContainerManager.Tick(d, TimeFlowManager.GetTimeAttenuation());
            }
            else
            {
                NPCAIManagerContainer.DisableAgents();
                //   NpcAiManager.DisableAgent();
            }

        }

        private void FixedUpdate()
        {
            var d = Time.fixedDeltaTime;
            PlayerManager.FixedTick(d);
        }

        private void OnDrawGizmos()
        {
            if (NPCAIManagerContainer != null)
            {
                NPCAIManagerContainer.GizmoTick();
            }
            /*
            if (NpcAiManager != null)
            {
                NpcAiManager.GizmoTick();
            }
            */

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

            /*
            if (NpcAiManager != null)
            {
                NpcAiManager.GUITick();
            }
            */

            if (PlayerActionManager != null)
            {
                PlayerActionManager.GUITick();
            }
        }
    }

}