using UnityEngine;
using UnityEngine.UI;

namespace AdventureGame
{

    public class InventoryCell : MonoBehaviour
    {
        private Item associatedItem;

        private Image inventoryCellImage;

        public Image InventoryCellImage { get => inventoryCellImage; }
        public Item AssociatedItem { get => associatedItem; }

        public void Init()
        {
            inventoryCellImage = GetComponent<Image>();
        }

        public void SetItem(Item item, ItemInherentData itemInherentData)
        {
            this.associatedItem = item;
            this.inventoryCellImage.sprite = itemInherentData.ItemMenuIcon;
        }

        public void ClearCell()
        {
            associatedItem = null;
            this.inventoryCellImage.sprite = null;
        }
    }

}