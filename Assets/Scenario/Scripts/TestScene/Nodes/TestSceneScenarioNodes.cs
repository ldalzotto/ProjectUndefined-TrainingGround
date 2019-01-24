using System.Collections.Generic;

public class IdCardGrabScenarioNode : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() {
        new RemoveGrabbableItem(ItemID.ID_CARD, PointOfInterestId.ID_CARD)
    };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() { new AddGrabbableItem(PointOfInterestId.ID_CARD,
        new AContextActionPOIBuilder((poi) => { return new GrabAction(ItemID.ID_CARD, poi); },
            new AContextActionPOIBuilder((poi) => { return new DummyContextAction(); }, null)))
    };

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
            {new GrabScenarioAction(ItemID.ID_CARD, PointOfInterestId.ID_CARD), new IdCardGiveScenarioNode() }
        };
}

public class IdCardGrabScenarioNodeV2 : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() { new RemoveGrabbableItem(ItemID.ID_CARD_V2, PointOfInterestId.ID_CARD_V2) };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() { new AddGrabbableItem(PointOfInterestId.ID_CARD_V2, new AContextActionPOIBuilder((poi) => { return new GrabAction(ItemID.ID_CARD_V2, poi); }, null)) };

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
          {new GrabScenarioAction(ItemID.ID_CARD_V2, PointOfInterestId.ID_CARD_V2), new IdCardGiveScenarioNodeV2() }
        };
}

public class IdCardGiveScenarioNode : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() { new RemoveReceivableItem(ItemID.ID_CARD, PointOfInterestId.BOUNCER) };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() { new AddReceivableItem(ItemID.ID_CARD, PointOfInterestId.BOUNCER) };

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
           {new GiveScenarioAction(ItemID.ID_CARD, PointOfInterestId.BOUNCER), null }
        };
}

public class IdCardGiveScenarioNodeV2 : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() { new RemoveReceivableItem(ItemID.ID_CARD_V2, PointOfInterestId.BOUNCER) };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() { new AddReceivableItem(ItemID.ID_CARD_V2, PointOfInterestId.BOUNCER) };

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
            {new GiveScenarioAction(ItemID.ID_CARD_V2, PointOfInterestId.BOUNCER), null }
        };
}