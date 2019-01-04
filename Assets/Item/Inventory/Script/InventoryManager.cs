using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private const string INVENTORY_ITEMS_CONTAINER_NAME = "InventoryItems";

    private HashSet<Item> holdItems = new HashSet<Item>();

    private InventoryExitTriggerManager InventoryExitTriggerManager;
    private InventoryStateWorkflowManager InventoryStateWorkflowManager;
    private InventoryMenu InventoryMenu;
    private InventoryItemManager InventoryItemManager;

    private GameObject InventoryItemsContainer;

    private void Start()
    {
        #region External dependencies
        var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        var InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
        #endregion

        InventoryItemsContainer = transform.Find(INVENTORY_ITEMS_CONTAINER_NAME).gameObject;

        InventoryMenu = GameObject.FindObjectOfType<InventoryMenu>();
        InventoryItemManager = new InventoryItemManager(InventoryItemsContainer);
        InventoryExitTriggerManager = new InventoryExitTriggerManager(GameInputManager, InventoryEventManager);
        InventoryStateWorkflowManager = new InventoryStateWorkflowManager();
    }


    public void Tick(float d)
    {
        if (IsInventoryMenuEnabled())
        {
            //TODO logic
            InventoryMenu.Tick(d);
            InventoryExitTriggerManager.Tick();
        }

        InventoryMenu.TickAnimation(d);
    }

    #region Logical Conditions
    private bool IsInventoryMenuEnabled()
    {
        return InventoryStateWorkflowManager.IsInventoryDisplayed;
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
        if (GameInputManager.CurrentInput.CancelButtonD())
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