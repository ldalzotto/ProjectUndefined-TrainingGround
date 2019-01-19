using System.Collections.Generic;

public class IdCardGrabScenarioNode : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() { new ScenarioTimelineEndAction() };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() { new ScenarioTimelineEnterAction() };

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
            {new GrabScenarioAction(ItemID.ID_CARD, PointOfInterestId.ID_CARD), new IdCardGiveScenarioNode() }
        };
}

public class IdCardGrabScenarioNodeV2 : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() { new ScenarioTimelineEndAction() };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() { new ScenarioTimelineEnterAction() };

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
          {new GrabScenarioAction(ItemID.ID_CARD_V2, PointOfInterestId.ID_CARD_V2), new IdCardGiveScenarioNodeV2() }
        };
}

public class IdCardGiveScenarioNode : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() { new ScenarioTimelineEndAction() };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() { new ScenarioTimelineEnterAction() };

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
           {new GiveScenarioAction(ItemID.ID_CARD, PointOfInterestId.BOUNCER), null }
        };
}

public class IdCardGiveScenarioNodeV2 : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() { new ScenarioTimelineEndAction() };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() { new ScenarioTimelineEnterAction() };

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
            {new GiveScenarioAction(ItemID.ID_CARD_V2, PointOfInterestId.BOUNCER), null }
        };
}