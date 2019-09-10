using CoreGame;
using GameConfigurationID;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public class InventoryManager : MonoBehaviour
    {
        private const string INVENTORY_ITEMS_CONTAINER_NAME = "InventoryItems";

        private List<ItemID> holdItems;

        #region External Dependencies
        private InventoryEventManager InventoryEventManager;
        #endregion

        private InventoryExitTriggerManager InventoryExitTriggerManager;
        private InventoryActionWheelTriggerManager InventoryActionWheelTriggerManager;
        private InventoryStateWorkflowManager InventoryStateWorkflowManager;
        private InventoryMenu InventoryMenu;
        private InventoryItemManager InventoryItemManager;
        private InventoryPersister InventoryPersister;

        private GameObject InventoryItemsContainer;

        public void Init()
        {
            Debug.Log(MyLog.Format("Inventory init"));
            #region External dependencies
            var GameInputManager = CoreGameSingletonInstances.GameInputManager;
            this.InventoryEventManager = AdventureGameSingletonInstances.InventoryEventManager;
            var ContextActionWheelEventManager = AdventureGameSingletonInstances.ContextActionWheelEventManager;
            var AdventureGameConfigurationManager = AdventureGameSingletonInstances.AdventureGameConfigurationManager;
            #endregion

            InventoryItemsContainer = transform.Find(INVENTORY_ITEMS_CONTAINER_NAME).gameObject;

            InventoryMenu = AdventureGameSingletonInstances.InventoryMenu;
            InventoryItemManager = new InventoryItemManager(InventoryItemsContainer);
            InventoryExitTriggerManager = new InventoryExitTriggerManager(GameInputManager, InventoryEventManager);
            InventoryStateWorkflowManager = new InventoryStateWorkflowManager();
            InventoryActionWheelTriggerManager = new InventoryActionWheelTriggerManager(GameInputManager, ContextActionWheelEventManager, InventoryStateWorkflowManager);

            InventoryPersister = new InventoryPersister();


            if (this.holdItems == null)
            {
                this.holdItems = new List<ItemID>();
                var persistedHoldItems = InventoryPersister.Load();
                if (persistedHoldItems == null)
                {
                    this.PersistCurrentItems();
                }
                else
                {
                    PersistInputItems(persistedHoldItems);
                }
            }

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
        public void OnAddItem(ItemID itemID, ItemInherentData itemInherentData, bool persistAddedItem = true)
        {
            if (!holdItems.Contains(itemID))
            {
                var itemGameObject = InventoryItemManager.OnItemAddInstanciatePrefab(itemID, itemInherentData);
                holdItems.Add(itemID);
                InventoryMenu.OnItemAdd(itemGameObject, itemInherentData);
                if (persistAddedItem)
                {
                    this.InventoryPersister.SaveAsync(this.holdItems);
                }
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
        public void OnItemGiven(ItemID itemID)
        {
            this.holdItems.Remove(itemID);
            StartCoroutine(InventoryItemManager.OnItemDelete(itemID, this.PersistCurrentItems));
        }

        #endregion


        private void PersistInputItems(List<ItemID> persistedHoldItems)
        {
            foreach (var loadedItem in persistedHoldItems)
            {
                this.InventoryEventManager.OnAddItem(loadedItem, persistAddedItem: false);
            }
            InventoryPersister.SaveAsync(this.holdItems);
        }

        private void PersistCurrentItems()
        {
            InventoryPersister.SaveAsync(this.holdItems);
        }
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
                    ContextActionWheelEventManager.OnWheelEnabled(currentSelectedItem.GetContextActions(), WheelTriggerSource.INVENTORY_MENU);
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
        #region External Dependencies
        private GameObject InventoryItemsContainer;
        #endregion

        public InventoryItemManager(GameObject inventoryItemsContainer)
        {
            InventoryItemsContainer = inventoryItemsContainer;
        }

        public Item OnItemAddInstanciatePrefab(ItemID itemID, ItemInherentData itemInherentData)
        {
            var itemPrefab = itemInherentData.ItemPrefab;
            var itemGameObject = MonoBehaviour.Instantiate(itemPrefab, InventoryItemsContainer.transform);
            itemGameObject.name = itemID.ToString();
            return itemGameObject;
        }

        public IEnumerator OnItemDelete(ItemID itemToDelete, Action inventoryPersistanceAction)
        {
            foreach (Transform possessedItemTransform in InventoryItemsContainer.transform)
            {
                Item possessedItem = possessedItemTransform.GetComponent<Item>();
                if (possessedItem.ItemID == itemToDelete)
                {
                    yield return new WaitForEndOfFrame();
                    MonoBehaviour.Destroy(possessedItem.gameObject);
                    inventoryPersistanceAction.Invoke();
                }
            }
        }
    }
    #endregion

    #region Inventory persistance
    class InventoryPersister : AbstractGamePersister<List<ItemID>>
    {
        public InventoryPersister() : base("Inventory", ".inv", "Inventory")
        {
        }
    }
    #endregion
}