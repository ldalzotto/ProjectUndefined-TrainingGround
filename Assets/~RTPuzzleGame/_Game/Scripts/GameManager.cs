﻿using UnityEngine;

namespace RTPuzzle
{

    public class GameManager : MonoBehaviour
    {

        public LevelZonesID PuzzleId;

        #region Persistance Dependencies
        private InventoryMenu InventoryMenu;
        #endregion

        private PlayerManager PlayerManager;
        private PlayerManagerDataRetriever PlayerManagerDataRetriever;
        private NpcAiManager NpcAiManager;
        private PlayerActionManager PlayerActionManager;
        private TimeFlowManager TimeFlowManager;
        private GroundEffectsManager GroundEffectsManager;

        private void Start()
        {
            InventoryMenu = GameObject.FindObjectOfType<InventoryMenu>();
            InventoryMenu.gameObject.SetActive(false);

            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            NpcAiManager = GameObject.FindObjectOfType<NpcAiManager>();
            PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
            PlayerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            TimeFlowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            GroundEffectsManager = GameObject.FindObjectOfType<GroundEffectsManager>();

            //Initialisations
            GameObject.FindObjectOfType<AIComponentsManager>().Init();
            PlayerManagerDataRetriever.Init();
            PlayerManager.Init();
            NpcAiManager.Init();
            TimeFlowManager.Init();
            GameObject.FindObjectOfType<PlayerActionEventManager>().Init();
            PlayerActionManager.Init(PuzzleId);
            GameObject.FindObjectOfType<LaunchProjectileContainerManager>().Init();
            GameObject.FindObjectOfType<LaunchProjectileEventManager>().Init();
            GroundEffectsManager.Init();

        }

        private void Update()
        {
            var d = Time.deltaTime;
            PlayerActionManager.Tick(d);
            PlayerManager.Tick(d);
            TimeFlowManager.Tick();
            GroundEffectsManager.Tick(d);
            if (TimeFlowManager.IsAbleToFlowTime())
            {
                NpcAiManager.EnableAgent();
                NpcAiManager.Tick(d, TimeFlowManager.GetTimeAttenuation());
            }
            else
            {
                NpcAiManager.DisableAgent();
            }

        }

        private void FixedUpdate()
        {
            var d = Time.fixedDeltaTime;
            PlayerManager.FixedTick(d);
        }

        private void OnDrawGizmos()
        {
            if (NpcAiManager != null)
            {
                NpcAiManager.GizmoTick();
            }

            if (PlayerActionManager != null)
            {
                PlayerActionManager.GizmoTick();
            }
        }

        private void OnGUI()
        {
            if (NpcAiManager != null)
            {
                NpcAiManager.GUITick();
            }

            if (PlayerActionManager != null)
            {
                PlayerActionManager.GUITick();
            }
        }
    }

}