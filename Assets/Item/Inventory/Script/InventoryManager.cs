﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private HashSet<Item> holdItems = new HashSet<Item>();

    public void AddItem(Item item)
    {
        holdItems.Add(item);
    }

    private InventoryExitTriggerManager InventoryExitTriggerManager;
    private InventoryStateWorkflowManager InventoryStateWorkflowManager;

    private void Start()
    {
        #region External dependencies
        var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        var InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
        #endregion

        InventoryExitTriggerManager = new InventoryExitTriggerManager(GameInputManager, InventoryEventManager);
        InventoryStateWorkflowManager = new InventoryStateWorkflowManager();
    }


    public void Tick(float d)
    {
        if (IsInventoryEnabled())
        {
            //TODO logic
            InventoryExitTriggerManager.Tick();
        }
    }

    #region Logical Conditions
    private bool IsInventoryEnabled()
    {
        return InventoryStateWorkflowManager.IsInventoryDisplayed;
    }
    #endregion

    #region External Events
    public IEnumerator OnInventoryEnabled()
    {
        yield return new WaitForEndOfFrame();
        InventoryStateWorkflowManager.OnInventoryEnabled();
    }
    public void OnInventoryDisabled()
    {
        InventoryStateWorkflowManager.OnInventoryDisabled();
    }
    #endregion
}

#region Inventory Workflow
class InventoryExitTriggerManager
{
    private GameInputManager GameInputManager;
    private InventoryEventManager InventoryEventManager;

    public InventoryExitTriggerManager(GameInputManager gameInputManager, InventoryEventManager inventoryEventManager)
    {
        GameInputManager = gameInputManager;
        InventoryEventManager = inventoryEventManager;
    }

    public void Tick()
    {
        if (GameInputManager.CurrentInput.InventoryButtonD())
        {
            InventoryEventManager.OnInventoryDisabled();
        }
    }
}

class InventoryStateWorkflowManager
{
    private bool isInventoryDisplayed;

    public bool IsInventoryDisplayed { get => isInventoryDisplayed; }

    public void OnInventoryEnabled()
    {
        isInventoryDisplayed = true;
    }
    public void OnInventoryDisabled()
    {
        isInventoryDisplayed = false;
    }

}
#endregion