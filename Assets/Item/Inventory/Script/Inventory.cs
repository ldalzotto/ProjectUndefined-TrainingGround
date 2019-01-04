using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const string INVENTORY_MENU_BODY_NAME = "InventoryMenu_Body";
    private const string INVENTORY_MENU_HEAD_NAME = "InventoryMenu_Head";

    public InventoryDimensionsComponent InventoryDimensionsComponent;

    private RectTransform InventoryMenuBody;
    private RectTransform InventoryMenuHead;

    private void Start()
    {
        InventoryMenuBody = (RectTransform)transform.Find(INVENTORY_MENU_BODY_NAME);
        InventoryMenuHead = (RectTransform)transform.Find(INVENTORY_MENU_HEAD_NAME);

        var bodyWidth = InventoryDimensionsComponent.ComputeInventoryWindowWidth();
        InventoryMenuBody.sizeDelta = new Vector2(bodyWidth, InventoryMenuBody.sizeDelta.y);
        InventoryMenuHead.localPosition = new Vector3(bodyWidth / 3, InventoryMenuHead.localPosition.y, InventoryMenuHead.localPosition.z);
    }

}

[System.Serializable]
public class InventoryDimensionsComponent
{
    public int MaxNumberOfItemPerRow;
    public float ItemIconWidth;
    public float BetweenItemSpace;
    public float ItemSelectionAreaBorder;

    public float ComputeInventoryWindowWidth()
    {
        return (MaxNumberOfItemPerRow * ItemIconWidth) + (BetweenItemSpace * (MaxNumberOfItemPerRow - 1)) + ItemSelectionAreaBorder * 2;
    }
}