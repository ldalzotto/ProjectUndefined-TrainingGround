﻿public class AddGrabbableItem : TimelineNodeWorkflowAction
{
    private PointOfInterestId poiInvolved;
    private ItemID itemID;
    private AContextAction contextAction;

    public AddGrabbableItem(ItemID itemID, PointOfInterestId poiInvolved, AContextAction contextAction)
    {
        this.itemID = itemID;
        this.poiInvolved = poiInvolved;
        this.contextAction = contextAction;
        this.contextAction.ContextActionWheelNodeConfigurationId = ContextActionWheelNodeConfigurationId.GRAB_CONTEXT_ACTION_WHEEL_CONFIG;
    }

    public override void Execute(PointOfInterestManager PointOfInterestManager, TimelineNode timelineNodeRefence)
    {
        var foundedPoi = PointOfInterestManager.GetActivePointOfInterest(poiInvolved);
        if (foundedPoi != null)
        {
            foundedPoi.OnGrabbableItemAdd(itemID, contextAction);
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