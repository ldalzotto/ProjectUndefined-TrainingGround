using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{

    public class InventoryEventManager : MonoBehaviour
    {

        private InventoryManager InventoryManager;
        private InventoryMenu InventoryMenu;
        private PlayerManager PlayerManager;

        #region External Dependencies
        private AdventureGameConfigurationManager AdventureGameConfigurationManager;
        #endregion

        public void Init()
        {
            InventoryManager = AdventureGameSingletonInstances.InventoryManager;
            InventoryMenu = AdventureGameSingletonInstances.InventoryMenu;
            PlayerManager = AdventureGameSingletonInstances.PlayerManager;
            this.AdventureGameConfigurationManager = AdventureGameSingletonInstances.AdventureGameConfigurationManager;
        }

        public void OnAddItem(ItemID itemID, bool persistAddedItem = true)
        {
            InventoryManager.OnAddItem(itemID, this.AdventureGameConfigurationManager.ItemConf()[itemID], persistAddedItem: persistAddedItem);
        }

        public void OnInventoryEnabled()
        {
            PlayerManager.OnInventoryEnabled();
            StartCoroutine(InventoryManager.OnInventoryEnabled());
            InventoryMenu.OnInventoryEnabled();
        }

        public void OnInventoryDisabled()
        {
            PlayerManager.OnInventoryDisabled();
            InventoryManager.OnInventoryDisabled();
            InventoryMenu.OnInventoryDisabled();
        }

        public void OnItemGiven(ItemID givenItem)
        {
            InventoryManager.OnItemGiven(givenItem);
            InventoryMenu.OnItemDeleted(givenItem);
        }

    }

}