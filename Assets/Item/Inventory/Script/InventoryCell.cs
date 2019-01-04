using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
    private Item associatedItem;

    private Image inventoryCellImage;

    public Image InventoryCellImage { get => inventoryCellImage; }

    public void Init()
    {
        inventoryCellImage = GetComponent<Image>();
    }
}
