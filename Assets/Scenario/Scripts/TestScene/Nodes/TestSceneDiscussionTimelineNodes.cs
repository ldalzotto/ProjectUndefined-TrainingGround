
using System.Collections.Generic;

public class BouncerKODiscussionNode : TimelineNode
{
    protected override List<TimelineNodeWorkflowAction> BuildExitDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>();
    }

    protected override List<TimelineNodeWorkflowAction> BuildStartDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>() {
             new DiscussionTimelineTreeCreationAction(PointOfInterestId.BOUNCER, new DiscussionTree(DiscussionSentencesConstants.Sentenses[DiscussionTreeId.BOUNCER_DISCUSSION_TREE]) )
        };
    }

    protected override Dictionary<ScenarioAction, TimelineNode> BuildTransitionRequirements()
    {
        return new Dictionary<ScenarioAction, TimelineNode>()
        {
              {new GiveScenarioAction(ItemID.ID_CARD, PointOfInterestId.BOUNCER), new BouncerOKDiscussioNode() }
        };
    }
}

public class BouncerOKDiscussioNode : TimelineNode
{
    protected override List<TimelineNodeWorkflowAction> BuildExitDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>();
    }

    protected override List<TimelineNodeWorkflowAction> BuildStartDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>()
        {
              new DiscussionTimelineTreeCreationAction(PointOfInterestId.BOUNCER, new DiscussionTree(DiscussionSentencesConstants.Sentenses[DiscussionTreeId.BOUNCER_OK_DISCUSSION]) )
        };
    }

    protected override Dictionary<ScenarioAction, TimelineNode> BuildTransitionRequirements()
    {
        return new Dictionary<ScenarioAction, TimelineNode>()
        {

        };
    }
}