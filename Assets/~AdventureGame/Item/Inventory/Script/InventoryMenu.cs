using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{

    public class InventoryMenu : AInventoryMenu
    {
        private const string INVENTORY_MENU_BODY_NAME = "InventoryMenu_Body";
        private const string INVENTORY_MENU_BODY_SELECTION_NAME = "InventoryMenu_ItemSelectionArea";
        private const string INVENTORY_MENU_BODY_CELL_CONTAINER = "InventoryMenu_CellContainer";
        private const string INVENTORY_MENU_HEAD_NAME = "InventoryMenu_Head";

        public InventoryDimensionsComponent InventoryDimensionsComponent;
        public InventoryAnimationManagerComponent InventoryAnimationManagerComponent;
        public InventoryItemSelectedTrackerManagerComponent InventoryItemSelectedTrackerManagerComponent;

        private InventoryCellContainer InventoryCellContainer;
        private InventoryItemSelectedTrackerManager InventoryItemSelectedTrackerManager;
        private InventoryAnimationManager InventoryAnimationManager;

        private RectTransform InventoryMenuBody;
        private RectTransform InventoryMenuBodySelectionArea;
        private RectTransform InventoryMenuBodyCellContainer;
        private RectTransform InventoryMenuHead;

        private bool hasBeenInit = false;

        public void Init()
        {
            if (!hasBeenInit)
            {
                #region External Dependencies
                var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
                #endregion

                InventoryAnimationManager = new InventoryAnimationManager(InventoryAnimationManagerComponent, (RectTransform)transform, InventoryDimensionsComponent);
                InventoryCellContainer = new InventoryCellContainer(InventoryDimensionsComponent.MaxNumberOfItemPerRow, InventoryDimensionsComponent.DisplayedRowNb);
                InventoryItemSelectedTrackerManager = new InventoryItemSelectedTrackerManager(GameInputManager, InventoryCellContainer, InventoryItemSelectedTrackerManagerComponent);

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
                for (var i = 0; i < InventoryDimensionsComponent.DisplayedRowNb * InventoryDimensionsComponent.MaxNumberOfItemPerRow; i++)
                {
                    var inventoryCell = Instantiate(PrefabContainer.Instance.InventoryMenuCellPrefab, InventoryMenuBodyCellContainer, false).GetComponent<InventoryCell>();
                    inventoryCell.Init();
                    ((RectTransform)inventoryCell.transform).sizeDelta = new Vector2(InventoryDimensionsComponent.ItemIconWidth, InventoryDimensionsComponent.ItemIconWidth);

                    var repeatedIndices = Mathf.Repeat(i, InventoryDimensionsComponent.MaxNumberOfItemPerRow);
                    var lineNb = Mathf.FloorToInt(i / InventoryDimensionsComponent.MaxNumberOfItemPerRow);
                    ((RectTransform)inventoryCell.transform).localPosition = new Vector3(
                     InventoryDimensionsComponent.ItemSelectionAreaBorder + (repeatedIndices * InventoryDimensionsComponent.ItemIconWidth) + Mathf.Max(0f, (repeatedIndices) * InventoryDimensionsComponent.BetweenItemSpace),
                      -lineNb * (InventoryDimensionsComponent.ItemIconWidth + InventoryDimensionsComponent.BetweenItemSpace),
                        0);
                    InventoryCellContainer.Set(i, inventoryCell);
                }

                InventoryItemSelectedTrackerManager.Init();

                //position inventory at the bottom of the screen
                var mainCanvasTransform = (RectTransform)transform.parent;
                transform.localPosition = new Vector3(0f, (-bodyFullHeight - mainCanvasTransform.sizeDelta.y) / 2, InventoryMenuBody.position.z);

                //initialize animations
                InventoryAnimationManager.InitializeAnimations((RectTransform)transform);

                InventoryMenuBody.gameObject.SetActive(true);
                InventoryMenuHead.gameObject.SetActive(true);

                hasBeenInit = true;
            }
           
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
            InventoryItemSelectedTrackerManager.Tick(d);
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
        public void OnItemAdd(Item item, ItemInherentData itemInherentData)
        {
            InventoryCellContainer.AddItemToFreeCell(item, itemInherentData);
        }
        public void OnItemDeleted(ItemID itemID)
        {
            InventoryCellContainer.DeleteItemFromMenu(itemID);
        }
        public Item GetCurrentSelectedItem()
        {
            return InventoryItemSelectedTrackerManager.GetCurrentItemSelected();
        }
        #endregion

        #region Functional conditions
        public bool IsAnimating()
        {
            return InventoryAnimationManager.IsAnimating;
        }
        #endregion

        public RectTransform GetInventoryHead()
        {
            return InventoryMenuHead;
        }

    }

    [System.Serializable]
    public class InventoryDimensionsComponent
    {
        public int MaxNumberOfItemPerRow;
        public int DisplayedRowNb;
        public float ItemIconWidth;
        public float BetweenItemSpace;
        public float ItemSelectionAreaBorder;
        public float DeltaDistanceFromBottomScreenWhenEnabled;

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

    #region Item Selected Tracker
    class InventoryItemSelectedTrackerManager
    {
        private GameInputManager GameInputManager;
        private InventoryCellContainer InventoryCellContainer;
        private InventoryItemSelectedTrackerManagerComponent InventoryItemSelectedTrackerManagerComponent;

        private InventoryCell currentlySelectedCell;
        private int selectedCellX;
        private int selectedCellY;

        private float timeElapsedFromLastSwitch;

        public InventoryItemSelectedTrackerManager(GameInputManager gameInputManager, InventoryCellContainer inventoryCellContainer, InventoryItemSelectedTrackerManagerComponent inventoryItemSelectedTrackerManagerComponent)
        {
            GameInputManager = gameInputManager;
            InventoryCellContainer = inventoryCellContainer;
            InventoryItemSelectedTrackerManagerComponent = inventoryItemSelectedTrackerManagerComponent;
        }

        public void Init()
        {
            ChangeCurrentlySelectedCell(0, 0);
        }

        private void ChangeCurrentlySelectedCell(int x, int y)
        {
            if (currentlySelectedCell != null)
            {
                currentlySelectedCell.InventoryCellImage.material = InventoryItemSelectedTrackerManagerComponent.InventoryItemNonSelectedMaterial;
            }
            currentlySelectedCell = InventoryCellContainer.GetCell(x, y);
            selectedCellX = x;
            selectedCellY = y;
            currentlySelectedCell.InventoryCellImage.material = InventoryItemSelectedTrackerManagerComponent.InventoryItemSelectedMaterial;
        }

        private void ChangeCurrentlySelectedIfExists(int x, int y)
        {
            if (InventoryCellContainer.CellExists(x, y))
            {
                ChangeCurrentlySelectedCell(x, y);
            }
        }

        public void Tick(float d)
        {
            if (timeElapsedFromLastSwitch >= InventoryItemSelectedTrackerManagerComponent.TimeBewteenCellSelectionSwitchThreshold)
            {
                timeElapsedFromLastSwitch = 0f;
                var locomotionAxis = GameInputManager.CurrentInput.LocomotionAxis();
                if (locomotionAxis.x >= 0.5)
                {
                    ChangeCurrentlySelectedIfExists(selectedCellX + 1, selectedCellY);
                }
                else if (locomotionAxis.x <= -0.5)
                {
                    ChangeCurrentlySelectedIfExists(selectedCellX - 1, selectedCellY);
                }
                else if (locomotionAxis.z >= 0.5)
                {
                    ChangeCurrentlySelectedIfExists(selectedCellX, selectedCellY - 1);
                }
                else if (locomotionAxis.z <= -0.5)
                {
                    ChangeCurrentlySelectedIfExists(selectedCellX, selectedCellY + 1);
                }
                else
                {
                    timeElapsedFromLastSwitch = InventoryItemSelectedTrackerManagerComponent.TimeBewteenCellSelectionSwitchThreshold * 2;
                }
            }
            else
            {
                timeElapsedFromLastSwitch += d;
            }

        }

        public Item GetCurrentItemSelected()
        {
            return currentlySelectedCell.AssociatedItem;
        }
    }

    [System.Serializable]
    public class InventoryItemSelectedTrackerManagerComponent
    {
        public Material InventoryItemSelectedMaterial;
        public Material InventoryItemNonSelectedMaterial;
        public float TimeBewteenCellSelectionSwitchThreshold;
    }
    #endregion

    #region Cell Container
    class InventoryCellContainer
    {
        private InventoryCell[] cells;

        private int width;
        private int height;

        public InventoryCellContainer(int width, int height)
        {
            this.cells = new InventoryCell[width * height];
            this.width = width;
            this.height = height;
        }

        public void Set(int i, InventoryCell inventoryCell)
        {
            cells[i] = inventoryCell;
        }

        public InventoryCell GetCell(int x, int y)
        {
            return cells[y * width + x];
        }

        public bool CellExists(int x, int y)
        {
            return (x >= 0 && x < width) && (y >= 0 && y < height);
        }

        public void AddItemToFreeCell(Item itemToAdd, ItemInherentData itemInherentData)
        {
            for (var h = 0; h < height; h++)
            {
                for (var w = 0; w < width; w++)
                {
                    var inventoryCell = GetCell(w, h);
                    if (inventoryCell.AssociatedItem == null)
                    {
                        inventoryCell.SetItem(itemToAdd, itemInherentData);
                        return;
                    }
                }
            }
        }

        public void DeleteItemFromMenu(ItemID deletedItem)
        {
            for (var h = 0; h < height; h++)
            {
                for (var w = 0; w < width; w++)
                {
                    var inventoryCell = GetCell(w, h);
                    if (inventoryCell.AssociatedItem != null && inventoryCell.AssociatedItem.ItemID == deletedItem)
                    {
                        inventoryCell.ClearCell();
                        return;
                    }
                }
            }
        }
    }
    #endregion

    #region Inventory Animation
    class InventoryAnimationManager
    {
        private float initialLocalY;
        private float finalLocalY;

        private float currentTargetLocalY;

        private InventoryAnimationManagerComponent InventoryAnimationManagerComponent;
        private RectTransform inventoryTransform;
        private InventoryDimensionsComponent InventoryDimensionsComponent;

        private bool isAnimating;

        public bool IsAnimating { get => isAnimating; }

        public InventoryAnimationManager(InventoryAnimationManagerComponent inventoryAnimationManagerComponent, RectTransform inventoryTransform, InventoryDimensionsComponent inventoryDimensionsComponent)
        {
            InventoryAnimationManagerComponent = inventoryAnimationManagerComponent;
            this.inventoryTransform = inventoryTransform;
            InventoryDimensionsComponent = inventoryDimensionsComponent;
        }

        public void InitializeAnimations(RectTransform inventoryTransform)
        {
            initialLocalY = inventoryTransform.anchoredPosition.y;
            finalLocalY = -initialLocalY - InventoryDimensionsComponent.DeltaDistanceFromBottomScreenWhenEnabled;
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
    #endregion

}