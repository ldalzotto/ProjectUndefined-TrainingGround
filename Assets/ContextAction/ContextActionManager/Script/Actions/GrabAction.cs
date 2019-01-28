using System.Collections;
using UnityEngine;

public class GrabAction : AContextAction
{
    public Item Item;
    private bool deletePOIOnGrab;

    private GrabActionInput grabActionInput;

    #region External Dependencies
    private InventoryEventManager InventoryEventManager;
    private PointOfInterestEventManager PointOfInterestEventManager;
    #endregion

    #region Internal Dependencies
    private PointOfInterestType associatedPOI;
    private ItemReceivedPopupManager ItemReceivedPopupManager;

    public PointOfInterestType AssociatedPOI { get => associatedPOI; }

    #endregion

    public GrabAction(ItemID itemId, bool deletePOIOnGrab, AContextAction nextAction) : base(nextAction)
    {
        Item = PrefabContainer.InventoryItemsPrefabs[itemId];

        #region External Dependencies
        InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
        PointOfInterestEventManager = GameObject.FindObjectOfType<PointOfInterestEventManager>();
        var GameCanvas = GameObject.FindObjectOfType<Canvas>();
        var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        ItemReceivedPopupManager = new ItemReceivedPopupManager(GameCanvas, GameInputManager, Item);

        this.deletePOIOnGrab = deletePOIOnGrab;
    }

    public override bool ComputeFinishedConditions()
    {
        return !isItemPopupOpen();
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        ItemReceivedPopupManager.ResetState();
        grabActionInput = (GrabActionInput)ContextActionInput;
        this.associatedPOI = grabActionInput.TargetedPOI;
    }

    public override void Tick(float d)
    {
        if (isItemPopupOpen())
        {
            ItemReceivedPopupManager.Tick(d);
        }
        else
        {
            InventoryEventManager.OnAddItem(grabActionInput.GrabbedItem);
        }
    }

    public override void AfterFinishedEventProcessed()
    {
        if (deletePOIOnGrab)
        {
            PointOfInterestEventManager.DestroyPOI(this.associatedPOI);
        }
    }

    #region Logical Conditions
    private bool isItemPopupOpen()
    {
        return ItemReceivedPopupManager.IsOpened;
    }
    #endregion
}

class ItemReceivedPopupManager
{
    private Canvas GameCanvas;
    private ItemReceivedPopup ItemReceivedPopup;
    private GameInputManager gameInputManager;
    private Item involvedItem;

    public ItemReceivedPopupManager(Canvas gameCanvas, GameInputManager gameInputManager, Item involvedItem)
    {
        this.GameCanvas = gameCanvas;
        this.gameInputManager = gameInputManager;
        this.involvedItem = involvedItem;
    }

    private bool isOpened;

    public bool IsOpened { get => isOpened; }

    public void Tick(float d)
    {
        ItemReceivedPopup.Tick(d);
        if (gameInputManager.CurrentInput.ActionButtonD())
        {
            ItemReceivedPopup.StartCoroutine(ExitPopup());
        }
    }

    private IEnumerator ExitPopup()
    {
        yield return ItemReceivedPopup.StartCoroutine(ItemReceivedPopup.OnClose());
        isOpened = false;
        Debug.Log("Destroy : " + ItemReceivedPopup.name);
        MonoBehaviour.Destroy(ItemReceivedPopup.gameObject);
    }

    public void ResetState()
    {
        ItemReceivedPopup = MonoBehaviour.Instantiate(PrefabContainer.Instance.ItemReceivedPopup, GameCanvas.transform);
        ItemReceivedPopup.Init(involvedItem);
        isOpened = true;
    }
}

public class GrabActionInput : AContextActionInput
{
    private PointOfInterestType targetedPOI;
    private Item grabbedItem;

    public GrabActionInput(PointOfInterestType targetedPOI, Item grabbedItem)
    {
        this.targetedPOI = targetedPOI;
        this.grabbedItem = grabbedItem;
    }

    public Item GrabbedItem { get => grabbedItem; }
    public PointOfInterestType TargetedPOI { get => targetedPOI; }
}