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
            InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
            InventoryMenu = GameObject.FindObjectOfType<InventoryMenu>();
            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            this.AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
        }

        public void OnAddItem(ItemID itemID)
        {
            InventoryManager.OnAddItem(itemID, this.AdventureGameConfigurationManager.ItemConf()[itemID]);
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