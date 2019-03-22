﻿using UnityEngine;

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
            NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            PlayerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            TimeFlowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            GroundEffectsManager = GameObject.FindObjectOfType<GroundEffectsManager>();
            LaunchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            CooldownFeedManager = GameObject.FindObjectOfType<CooldownFeedManager>();
            TimeFlowPlayPauseManager = GameObject.FindObjectOfType<TimeFlowPlayPauseManager>();
            AttractiveObjectsContainerManager = GameObject.FindObjectOfType<AttractiveObjectsContainerManager>();
            
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var TimeFlowBarManager = GameObject.FindObjectOfType<TimeFlowBarManager>();
            var LevelManager = GameObject.FindObjectOfType<LevelManager>();
            var PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();

            //Initialisations
            var targetZones = GameObject.FindObjectsOfType<TargetZone>();
            if (targetZones != null)
            {
                foreach (var targetZone in targetZones)
                {
                    targetZone.Init();
                }
            }

            GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().Init();
            PlayerManagerDataRetriever.Init();
            PlayerManager.Init();
            TimeFlowBarManager.Init(puzzleConfigurationManager.LevelConfiguration()[PuzzleId].AvailableTimeAmount);
            TimeFlowManager.Init(PuzzleId, PlayerManagerDataRetriever, PlayerManager, gameInputManager, puzzleConfigurationManager, TimeFlowBarManager, LevelManager, PuzzleEventsManager);
            GameObject.FindObjectOfType<PlayerActionEventManager>().Init();
            PlayerActionManager.Init(PuzzleId);
            LaunchProjectileContainerManager.Init();
            GameObject.FindObjectOfType<LaunchProjectileEventManager>().Init();
            GameObject.FindObjectOfType<GroundCollision>().Init();
            GroundEffectsManager.Init();
            CooldownFeedManager.Init();
            PuzzleEventsManager.Init();
            TimeFlowPlayPauseManager.Init();
            GameObject.FindObjectOfType<PlayerActionPuzzleEventsManager>().Init();
            LevelManager.Init(PuzzleId);
            AttractiveObjectsContainerManager.Init();
            GameObject.FindObjectOfType<NpcInteractionRingContainer>().Init();
            GameObject.FindObjectOfType<NpcInteractionRingRendererManager>().Init();
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