using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private const string INVENTORY_ITEMS_CONTAINER_NAME = "InventoryItems";

    private HashSet<Item> holdItems = new HashSet<Item>();

    private InventoryExitTriggerManager InventoryExitTriggerManager;
    private InventoryActionWheelTriggerManager InventoryActionWheelTriggerManager;
    private InventoryStateWorkflowManager InventoryStateWorkflowManager;
    private InventoryMenu InventoryMenu;
    private InventoryItemManager InventoryItemManager;

    private GameObject InventoryItemsContainer;

    private void Start()
    {
        #region External dependencies
        var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        var InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
        var ContextActionWheelEventManager = GameObject.FindObjectOfType<ContextActionWheelEventManager>();
        #endregion

        InventoryItemsContainer = transform.Find(INVENTORY_ITEMS_CONTAINER_NAME).gameObject;

        InventoryMenu = GameObject.FindObjectOfType<InventoryMenu>();
        InventoryItemManager = new InventoryItemManager(InventoryItemsContainer);
        InventoryExitTriggerManager = new InventoryExitTriggerManager(GameInputManager, InventoryEventManager);
        InventoryStateWorkflowManager = new InventoryStateWorkflowManager();
        InventoryActionWheelTriggerManager = new InventoryActionWheelTriggerManager(GameInputManager, ContextActionWheelEventManager, InventoryStateWorkflowManager);
    }


    public void Tick(float d)
    {
        if (IsInventoryMenuEnabled())
        {
            if (!IsInventoryActionWheelEnabled())
            {
                //If exit button is not pressed
                if (!InventoryExitTriggerManager.Tick())
                {
                    InventoryMenu.Tick(d);
                    InventoryActionWheelTriggerManager.Tick(InventoryMenu.GetCurrentSelectedItem());
                }
            }

        }

        InventoryMenu.TickAnimation(d);
    }

    #region Logical Conditions
    private bool IsInventoryMenuEnabled()
    {
        return InventoryStateWorkflowManager.IsInventoryDisplayed;
    }
    private bool IsInventoryActionWheelEnabled()
    {
        return InventoryStateWorkflowManager.IsInventoryActionWheelDisplayed;
    }
    #endregion

    #region External Events
    public void OnAddItem(Item item)
    {
        if (holdItems.Add(item))
        {
            var itemGameObject = InventoryItemManager.OnItemAddInstanciatePrefab(item);
            InventoryMenu.OnItemAdd(itemGameObject);
        }
    }
    public IEnumerator OnInventoryEnabled()
    {
        yield return new WaitForEndOfFrame();
        InventoryStateWorkflowManager.OnInventoryEnabled();
    }
    public void OnInventoryDisabled()
    {
        InventoryStateWorkflowManager.OnInventoryDisabled();
    }
    public IEnumerator OnContextActionWheelDisabled()
    {
        yield return new WaitForEndOfFrame();
        InventoryStateWorkflowManager.IsInventoryActionWheelDisplayed = false;
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

    public bool Tick()
    {
        if (GameInputManager.CurrentInput.CancelButtonD())
        {
            InventoryEventManager.OnInventoryDisabled();
            return true;
        }
        return false;
    }
}

class InventoryActionWheelTriggerManager
{
    private GameInputManager GameInputManager;
    private ContextActionWheelEventManager ContextActionWheelEventManager;
    private InventoryStateWorkflowManager InventoryStateWorkflowManager;

    public InventoryActionWheelTriggerManager(GameInputManager gameInputManager, ContextActionWheelEventManager contextActionWheelEventManager, InventoryStateWorkflowManager InventoryStateWorkflowManager)
    {
        GameInputManager = gameInputManager;
        ContextActionWheelEventManager = contextActionWheelEventManager;
        this.InventoryStateWorkflowManager = InventoryStateWorkflowManager;
    }

    public bool Tick(Item currentSelectedItem)
    {
        if (currentSelectedItem != null)
        {
            if (GameInputManager.CurrentInput.ActionButtonD())
            {
                ContextActionWheelEventManager.OnWheelEnabled(currentSelectedItem.ContextActions, WheelTriggerSource.INVENTORY_MENU);
                InventoryStateWorkflowManager.IsInventoryActionWheelDisplayed = true;
                return true;
            }
        }

        return false;
    }
}

class InventoryStateWorkflowManager
{
    private bool isInventoryDisplayed;
    private bool isInventoryActionWheelDisplayed;

    public bool IsInventoryDisplayed { get => isInventoryDisplayed; }
    public bool IsInventoryActionWheelDisplayed { get => isInventoryActionWheelDisplayed; set => isInventoryActionWheelDisplayed = value; }

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

#region Inventory Item
class InventoryItemManager
{

    private GameObject InventoryItemsContainer;

    public InventoryItemManager(GameObject inventoryItemsContainer)
    {
        InventoryItemsContainer = inventoryItemsContainer;
    }

    public Item OnItemAddInstanciatePrefab(Item item)
    {
        var itemGameObject = MonoBehaviour.Instantiate(item, InventoryItemsContainer.transform);
        itemGameObject.name = item.name;
        return itemGameObject;
    }
}
#endregion