using UnityEngine;

public interface DiscussionTimelineModifierAction
{
    void Execute(PointOfInterestManager PointOfInterestManager);
}

public class DiscussionTimelineTreeCreationAction : DiscussionTimelineModifierAction
{
    private PointOfInterestId PointOfInterestId;
    private DiscussionTree DiscussionTree;

    public DiscussionTimelineTreeCreationAction(PointOfInterestId pointOfInterestId, DiscussionTree DiscussionTree)
    {
        PointOfInterestId = pointOfInterestId;
        this.DiscussionTree = DiscussionTree;
    }

    public void Execute(PointOfInterestManager PointOfInterestManager)
    {
        var selectedPOI = PointOfInterestManager.GetActivePointOfInterest(PointOfInterestId);
        if (selectedPOI != null)
        {
            selectedPOI.OnDiscussionTreeAdd(DiscussionTree);
        }
    }
}

public class DiscussionTimelineTreeChoiceDeleteAction : DiscussionTimelineModifierAction
{
    private PointOfInterestId PointOfInterestId;
    private DiscussionChoiceTextId DiscussionIdToDelete;

    public DiscussionTimelineTreeChoiceDeleteAction(PointOfInterestId pointOfInterestId, DiscussionChoiceTextId discussionIdToDelete)
    {
        PointOfInterestId = pointOfInterestId;
        DiscussionIdToDelete = discussionIdToDelete;
    }

    public void Execute(PointOfInterestManager PointOfInterestManager)
    {
        var selectedPOI = PointOfInterestManager.GetActivePointOfInterest(PointOfInterestId);
        if (selectedPOI != null)
        {
            var discussionTree = selectedPOI.GetAssociatedDiscussionTree();
            Debug.Log("//TODO -> DELETE");
        }
    }
}