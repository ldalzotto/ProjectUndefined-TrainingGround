using System.Collections.Generic;

public class IdCardGrabScenarioNode : TimelineNode
{
    protected override List<TimelineNodeWorkflowAction> BuildExitDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>()
        {
            new ScenarioTimelineEndAction()
        };
    }

    protected override List<TimelineNodeWorkflowAction> BuildStartDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>()
        {
            new ScenarioTimelineEnterAction()
        };
    }

    protected override Dictionary<ScenarioAction, TimelineNode> BuildTransitionRequirements()
    {
        return new Dictionary<ScenarioAction, TimelineNode>()
        {
            {new GrabScenarioAction(ItemID.ID_CARD, PointOfInterestId.ID_CARD), new IdCardGiveScenarioNode() }
        };
    }
}

public class IdCardGrabScenarioNodeV2 : TimelineNode
{
    protected override List<TimelineNodeWorkflowAction> BuildExitDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>()
        {
            new ScenarioTimelineEndAction()
        };
    }

    protected override List<TimelineNodeWorkflowAction> BuildStartDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>()
        {
            new ScenarioTimelineEnterAction()
        };
    }

    protected override Dictionary<ScenarioAction, TimelineNode> BuildTransitionRequirements()
    {
        return new Dictionary<ScenarioAction, TimelineNode>()
        {
            {new GrabScenarioAction(ItemID.ID_CARD_V2, PointOfInterestId.ID_CARD_V2), new IdCardGiveScenarioNodeV2() }
        };
    }
}

public class IdCardGiveScenarioNode : TimelineNode
{
    protected override List<TimelineNodeWorkflowAction> BuildExitDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>()
        {
            new ScenarioTimelineEndAction()
        };
    }

    protected override List<TimelineNodeWorkflowAction> BuildStartDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>()
        {
            new ScenarioTimelineEnterAction()
        };
    }

    protected override Dictionary<ScenarioAction, TimelineNode> BuildTransitionRequirements()
    {
        return new Dictionary<ScenarioAction, TimelineNode>()
        {
            {new GiveScenarioAction(ItemID.ID_CARD, PointOfInterestId.BOUNCER), null }
        };
    }
}

public class IdCardGiveScenarioNodeV2 : TimelineNode
{
    protected override List<TimelineNodeWorkflowAction> BuildExitDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>()
        {
            new ScenarioTimelineEndAction()
        };
    }

    protected override List<TimelineNodeWorkflowAction> BuildStartDiscussionTreeActions()
    {
        return new List<TimelineNodeWorkflowAction>()
        {
            new ScenarioTimelineEnterAction()
        };
    }

    protected override Dictionary<ScenarioAction, TimelineNode> BuildTransitionRequirements()
    {
        return new Dictionary<ScenarioAction, TimelineNode>()
        {
            {new GiveScenarioAction(ItemID.ID_CARD_V2, PointOfInterestId.BOUNCER), null }
        };
    }
}