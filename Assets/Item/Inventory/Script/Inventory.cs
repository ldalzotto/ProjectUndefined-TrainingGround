using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const string INVENTORY_MENU_BODY_NAME = "InventoryMenu_Body";
    private const string INVENTORY_MENU_BODY_SELECTION_NAME = "InventoryMenu_ItemSelectionArea";
    private const string INVENTORY_MENU_BODY_CELL_CONTAINER = "InventoryMenu_CellContainer";
    private const string INVENTORY_MENU_HEAD_NAME = "InventoryMenu_Head";

    public InventoryDimensionsComponent InventoryDimensionsComponent;
    public InventoryAnimationManagerComponent InventoryAnimationManagerComponent;

    private InventoryAnimationManager InventoryAnimationManager;

    private RectTransform InventoryMenuBody;
    private RectTransform InventoryMenuBodySelectionArea;
    private RectTransform InventoryMenuBodyCellContainer;
    private RectTransform InventoryMenuHead;

    private InventoryCell[] inventoryCells;

    private void Start()
    {
        InventoryAnimationManager = new InventoryAnimationManager(InventoryAnimationManagerComponent, (RectTransform)transform);

        InventoryMenuBody = (RectTransform)transform.Find(INVENTORY_MENU_BODY_NAME);
        InventoryMenuBodySelectionArea = (RectTransform)InventoryMenuBody.Find(INVENTORY_MENU_BODY_SELECTION_NAME);
        InventoryMenuBodyCellContainer = (RectTransform)InventoryMenuBodySelectionArea.Find(INVENTORY_MENU_BODY_CELL_CONTAINER);
        InventoryMenuHead = (RectTransform)transform.Find(INVENTORY_MENU_HEAD_NAME);

        var bodyFullWidth = InventoryDimensionsComponent.ComputeInventoryWindowFullWidth();
        var bodyFullHeight = InventoryDimensionsComponent.ComputeInventoryWindowFullHeight();

        InventoryMenuBody.sizeDelta = new Vector2(bodyFullWidth, bodyFullHeight);
        InventoryMenuBodySelectionArea.sizeDelta = new Vector2(
            InventoryDimensionsComponent.ComputeInventoryWindowSelectionAreaWidth(),
            InventoryDimensionsComponent.ComputeInventoryWindowSelectionAreaHeight());
        InventoryMenuHead.localPosition = new Vector3(bodyFullWidth / 3, bodyFullHeight / 2, InventoryMenuHead.localPosition.z);


        //initialize cells
        InventoryMenuBodyCellContainer.localPosition = new Vector3(InventoryMenuBodySelectionArea.rect.xMin + InventoryDimensionsComponent.ItemIconWidth / 2, InventoryMenuBodySelectionArea.rect.yMax - (InventoryDimensionsComponent.ItemIconWidth / 2) - InventoryDimensionsComponent.ItemSelectionAreaBorder, 0);
        inventoryCells = new InventoryCell[InventoryDimensionsComponent.DisplayedRowNb * InventoryDimensionsComponent.MaxNumberOfItemPerRow];
        for (var i = 0; i < InventoryDimensionsComponent.DisplayedRowNb * InventoryDimensionsComponent.MaxNumberOfItemPerRow; i++)
        {
            var inventoryCell = Instantiate(PrefabContainer.Instance.InventoryMenuCellPrefab, InventoryMenuBodyCellContainer, false).GetComponent<InventoryCell>();
            ((RectTransform)inventoryCell.transform).sizeDelta = new Vector2(InventoryDimensionsComponent.ItemIconWidth, InventoryDimensionsComponent.ItemIconWidth);

            var repeatedIndices = Mathf.Repeat(i, InventoryDimensionsComponent.MaxNumberOfItemPerRow);
            var lineNb = Mathf.FloorToInt(i / InventoryDimensionsComponent.MaxNumberOfItemPerRow);
            ((RectTransform)inventoryCell.transform).localPosition = new Vector3(
             InventoryDimensionsComponent.ItemSelectionAreaBorder + (repeatedIndices * InventoryDimensionsComponent.ItemIconWidth) + Mathf.Max(0f, (repeatedIndices) * InventoryDimensionsComponent.BetweenItemSpace),
              -lineNb * (InventoryDimensionsComponent.ItemIconWidth + InventoryDimensionsComponent.BetweenItemSpace),
                0);
            inventoryCells[i] = inventoryCell;
        }

        //position inventory at the bottom of the screen
        var mainCanvasTransform = (RectTransform)transform.parent;
        transform.localPosition = new Vector3(0f, (-bodyFullHeight - mainCanvasTransform.sizeDelta.y) / 2, InventoryMenuBody.position.z);

        //initialize animations
        InventoryAnimationManager.InitializeAnimations((RectTransform)transform);

        InventoryMenuBody.gameObject.SetActive(true);
        InventoryMenuHead.gameObject.SetActive(true);

    }

    public void TickAnimation(float d)
    {
        if (IsAnimating())
        {
            InventoryAnimationManager.Tick(d);
        }
    }

    public void Tick(float d)
    {
    }

    #region External Events
    public void OnInventoryEnabled()
    {
        InventoryAnimationManager.OnInventoryEnabled();
    }
    public void OnInventoryDisabled()
    {
        InventoryAnimationManager.OnInventoryDisabled();
    }
    #endregion

    #region Functional conditions
    public bool IsAnimating()
    {
        return InventoryAnimationManager.IsAnimating;
    }
    #endregion
}

[System.Serializable]
public class InventoryDimensionsComponent
{
    public int MaxNumberOfItemPerRow;
    public int DisplayedRowNb;
    public float ItemIconWidth;
    public float BetweenItemSpace;
    public float ItemSelectionAreaBorder;

    public float ComputeInventoryWindowFullWidth()
    {
        return (MaxNumberOfItemPerRow * ItemIconWidth) + (BetweenItemSpace * (MaxNumberOfItemPerRow - 1)) + ItemSelectionAreaBorder * 4;
    }

    public float ComputeInventoryWindowFullHeight()
    {
        return (DisplayedRowNb * ItemIconWidth) + (BetweenItemSpace * (DisplayedRowNb - 1)) + ItemSelectionAreaBorder * 4;
    }

    public float ComputeInventoryWindowSelectionAreaWidth()
    {
        return (MaxNumberOfItemPerRow * ItemIconWidth) + (BetweenItemSpace * (MaxNumberOfItemPerRow - 1)) + ItemSelectionAreaBorder * 2;
    }

    public float ComputeInventoryWindowSelectionAreaHeight()
    {
        return (DisplayedRowNb * ItemIconWidth) + (BetweenItemSpace * (DisplayedRowNb - 1)) + ItemSelectionAreaBorder * 2;
    }

}

class InventoryAnimationManager
{
    private float initialLocalY;
    private float finalLocalY;

    private float currentTargetLocalY;

    private InventoryAnimationManagerComponent InventoryAnimationManagerComponent;
    private RectTransform inventoryTransform;

    private bool isAnimating;

    public bool IsAnimating { get => isAnimating; }

    public InventoryAnimationManager(InventoryAnimationManagerComponent inventoryAnimationManagerComponent, RectTransform inventoryTransform)
    {
        InventoryAnimationManagerComponent = inventoryAnimationManagerComponent;
        this.inventoryTransform = inventoryTransform;
    }

    public void InitializeAnimations(RectTransform inventoryTransform)
    {
        initialLocalY = inventoryTransform.anchoredPosition.y;
        finalLocalY = -initialLocalY;
        currentTargetLocalY = initialLocalY;
    }

    public void OnInventoryEnabled()
    {
        isAnimating = true;
        currentTargetLocalY = finalLocalY;
    }

    public void OnInventoryDisabled()
    {
        isAnimating = true;
        currentTargetLocalY = initialLocalY;
    }

    public void Tick(float d)
    {
        this.inventoryTransform.anchoredPosition = new Vector2(inventoryTransform.anchoredPosition.x, Mathf.Lerp(inventoryTransform.anchoredPosition.y, currentTargetLocalY, d * InventoryAnimationManagerComponent.TransitionSpeed));

        if (Mathf.Abs(inventoryTransform.anchoredPosition.y - currentTargetLocalY) <= 0.001f)
        {
            this.isAnimating = false;
            this.inventoryTransform.anchoredPosition = new Vector2(inventoryTransform.anchoredPosition.x, currentTargetLocalY);
        }
    }

}

[System.Serializable]
public class InventoryAnimationManagerComponent
{
    public float TransitionSpeed;
}

