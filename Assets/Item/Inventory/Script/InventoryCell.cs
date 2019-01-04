using UnityEngine;
using UnityEngine.UI;

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

    public void SetItem(Item item)
    {
        this.associatedItem = item;
        this.RefreshItemSprite();
    }

    public void RefreshItemSprite()
    {
        inventoryCellImage.sprite = ItemResourceResolver.ResolveItemInventoryIcon(associatedItem);
    }
}
