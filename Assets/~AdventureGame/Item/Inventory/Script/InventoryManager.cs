﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public class InventoryManager : MonoBehaviour
    {
        private const string INVENTORY_ITEMS_CONTAINER_NAME = "InventoryItems";

        private Dictionary<ItemID, Item> holdItems = new Dictionary<ItemID, Item>();

        private InventoryExitTriggerManager InventoryExitTriggerManager;
        private InventoryActionWheelTriggerManager InventoryActionWheelTriggerManager;
        private InventoryStateWorkflowManager InventoryStateWorkflowManager;
        private InventoryMenu InventoryMenu;
        private InventoryItemManager InventoryItemManager;

        private GameObject InventoryItemsContainer;

        public void Init()
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
                if (IsInventoryInteractable())
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
        private bool IsInventoryInteractable()
        {
            return !InventoryStateWorkflowManager.IsInventoryActionWheelDisplayed && !InventoryStateWorkflowManager.IsWontextActionRuning;
        }
        #endregion

        #region External Events
        public void OnAddItem(Item item)
        {
            if (!holdItems.ContainsKey(item.ItemID))
            {
                var itemGameObject = InventoryItemManager.OnItemAddInstanciatePrefab(item);
                holdItems[item.ItemID] = itemGameObject;
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
        public void OnContextActionAdded()
        {
            InventoryStateWorkflowManager.OnContextActionAdded();
        }
        public void OnContextActionFinished()
        {
            InventoryStateWorkflowManager.OnContextActionFinished();
        }
        public void OnItemGiven(Item item)
        {
            StartCoroutine(InventoryItemManager.OnItemDelete(item));
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
        private bool isContextActionRuning;

        public bool IsInventoryDisplayed { get => isInventoryDisplayed; }
        public bool IsInventoryActionWheelDisplayed { get => isInventoryActionWheelDisplayed; set => isInventoryActionWheelDisplayed = value; }
        public bool IsWontextActionRuning { get => isContextActionRuning; }

        public void OnInventoryEnabled()
        {
            isInventoryDisplayed = true;
        }
        public void OnInventoryDisabled()
        {
            isInventoryDisplayed = false;
        }
        public void OnContextActionAdded()
        {
            isContextActionRuning = true;
        }
        public void OnContextActionFinished()
        {
            isContextActionRuning = false;
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

        public IEnumerator OnItemDelete(Item itemToDelete)
        {
            foreach (Transform possessedItemTransform in InventoryItemsContainer.transform)
            {
                Item possessedItem = possessedItemTransform.GetComponent<Item>();
                if (possessedItem.ItemID == itemToDelete.ItemID)
                {
                    yield return new WaitForEndOfFrame();
                    MonoBehaviour.Destroy(itemToDelete.gameObject);
                }
            }
        }
    }
    #endregion
}