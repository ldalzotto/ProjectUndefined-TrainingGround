﻿using UnityEngine;

public class RTPuzzleGameManager : MonoBehaviour
{

    #region Persistance Dependencies
    private InventoryMenu InventoryMenu;
    #endregion

    private RTPlayerManager RTPlayerManager;
    private RTP_NPCManager RTP_NPCManager;

    private void Start()
    {
        InventoryMenu = GameObject.FindObjectOfType<InventoryMenu>();
        InventoryMenu.gameObject.SetActive(false);

        RTPlayerManager = GameObject.FindObjectOfType<RTPlayerManager>();
        RTP_NPCManager = GameObject.FindObjectOfType<RTP_NPCManager>();

        //Initialisations
        GameObject.FindObjectOfType<RTPlayerManagerDataRetriever>().Init();
        RTPlayerManager.Init();
        RTP_NPCManager.Init();
    }

    private void Update()
    {
        var d = Time.deltaTime;
        RTPlayerManager.Tick(d);
        if (RTPlayerManager.HasPlayerMovedThisFrame())
        {
            RTP_NPCManager.EnableAgent();
            RTP_NPCManager.Tick(d);
        }
        else
        {
            RTP_NPCManager.DisableAgent();
        }

    }

    private void FixedUpdate()
    {
        var d = Time.fixedDeltaTime;
        RTPlayerManager.FixedTick(d);
    }

    private void OnDrawGizmos()
    {
        if (RTP_NPCManager != null)
        {
            RTP_NPCManager.GizmoTick();
        }

    }
}
