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

    public void ClearCell()
    {
        associatedItem = null;
        this.RefreshItemSprite();
    }

    public void RefreshItemSprite()
    {
        if (associatedItem != null)
        {
            inventoryCellImage.sprite = ItemResourceResolver.ResolveItemInventoryIcon(associatedItem);
        }
        else
        {
            inventoryCellImage.sprite = null;
        }

    }
}
