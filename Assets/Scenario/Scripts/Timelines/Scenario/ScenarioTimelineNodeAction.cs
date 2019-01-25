public class AddGrabbableItem : TimelineNodeWorkflowAction
{
    private PointOfInterestId poiInvolved;
    private AContextAction contextAction;

    public AddGrabbableItem(PointOfInterestId poiInvolved, AContextAction contextAction)
    {
        this.poiInvolved = poiInvolved;
        this.contextAction = contextAction;
    }

    public override void Execute(PointOfInterestManager PointOfInterestManager, TimelineNode timelineNodeRefence)
    {
        var foundedPoi = PointOfInterestManager.GetActivePointOfInterest(poiInvolved);
        if (foundedPoi != null)
        {
            foundedPoi.OnGrabbableItemAdd(contextAction);
        }
    }
}

public class RemoveGrabbableItem : TimelineNodeWorkflowAction
{
    private ItemID itemInvolved;
    private PointOfInterestId poiInvolved;

    public RemoveGrabbableItem(ItemID itemInvolved, PointOfInterestId poiInvolved)
    {
        this.itemInvolved = itemInvolved;
        this.poiInvolved = poiInvolved;
    }

    public override void Execute(PointOfInterestManager PointOfInterestManager, TimelineNode timelineNodeRefence)
    {
        var foundedPoi = PointOfInterestManager.GetActivePointOfInterest(poiInvolved);
        if (foundedPoi != null)
        {
            foundedPoi.OnGrabbableItemRemove(itemInvolved);
        }
    }
}

public class AddReceivableItem : TimelineNodeWorkflowAction
{
    private ItemID itemInvolved;
    private PointOfInterestId poiInvolved;

    public AddReceivableItem(ItemID itemInvolved, PointOfInterestId poiInvolved)
    {
        this.itemInvolved = itemInvolved;
        this.poiInvolved = poiInvolved;
    }

    public override void Execute(PointOfInterestManager PointOfInterestManager, TimelineNode timelineNodeRefence)
    {
        var foundedPoi = PointOfInterestManager.GetActivePointOfInterest(poiInvolved);
        if (foundedPoi != null)
        {
            foundedPoi.OnReceivableItemAdd(itemInvolved);
        }
    }
}

public class RemoveReceivableItem : TimelineNodeWorkflowAction
{
    private ItemID itemInvolved;
    private PointOfInterestId poiInvolved;

    public RemoveReceivableItem(ItemID itemInvolved, PointOfInterestId poiInvolved)
    {
        this.itemInvolved = itemInvolved;
        this.poiInvolved = poiInvolved;
    }

    public override void Execute(PointOfInterestManager PointOfInterestManager, TimelineNode timelineNodeRefence)
    {
        var foundedPoi = PointOfInterestManager.GetActivePointOfInterest(poiInvolved);
        if (foundedPoi != null)
        {
            foundedPoi.OnReceivableItemRemove(itemInvolved);
        }
    }
}