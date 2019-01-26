using UnityEngine;

public class GrabAction : AContextAction
{
    public Item Item;
    private bool deletePOIOnGrab;

    private GrabActionInput grabActionInput;

    private InventoryEventManager InventoryEventManager;
    private PointOfInterestEventManager PointOfInterestEventManager;

    #region Internal Dependencies
    private PointOfInterestType associatedPOI;

    public PointOfInterestType AssociatedPOI { get => associatedPOI; }

    #endregion

    public GrabAction(ItemID itemId, bool deletePOIOnGrab, AContextAction nextAction) : base(nextAction)
    {
        Item = PrefabContainer.InventoryItemsPrefabs[itemId];
        InventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
        PointOfInterestEventManager = GameObject.FindObjectOfType<PointOfInterestEventManager>();

        this.deletePOIOnGrab = deletePOIOnGrab;
    }

    public override bool ComputeFinishedConditions()
    {
        return true;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        grabActionInput = (GrabActionInput)ContextActionInput;
        this.associatedPOI = grabActionInput.TargetedPOI;
        InventoryEventManager.OnAddItem(grabActionInput.GrabbedItem);
    }

    public override void Tick(float d)
    {

    }

    public override void AfterFinishedEventProcessed()
    {
        if (deletePOIOnGrab)
        {
            PointOfInterestEventManager.DestroyPOI(this.associatedPOI);
        }
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