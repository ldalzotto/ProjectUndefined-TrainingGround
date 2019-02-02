
using System.Collections.Generic;

#region ScenarioNode
public class CrowbarScenarioNode : TimelineNode
{
    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>() {
        { new GrabScenarioAction(ItemID.CROWBAR, PointOfInterestId.CROWBAR), new SewerEntranceScenarioNode()}
    };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>()
    {
        new AddGrabbableItem(ItemID.CROWBAR, PointOfInterestId.CROWBAR, new AnimatorAction(PlayerAnimatioNamesEnum.PLAYER_ACTION_GRAB_DOWN,  new GrabAction(ItemID.CROWBAR, true, null)))
    };

    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>();
}

public class SewerEntranceScenarioNode : TimelineNode
{
    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>();

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>()
    {
        new AddPOIInteractableItem(ItemID.CROWBAR, PointOfInterestId.SEWER_ENTRANCE)
    };

    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>();
}

public class IdCardGrabScenarioNode : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() {
        new RemoveGrabbableItem(ItemID.ID_CARD, PointOfInterestId.ID_CARD)
    };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() {
        new AddGrabbableItem(ItemID.ID_CARD, PointOfInterestId.ID_CARD,
            new AnimatorAction( PlayerAnimatioNamesEnum.PLAYER_ACTION_GRAB_DOWN,
                new GrabAction(ItemID.ID_CARD, true,
                   new DummyContextAction(null)
                )
            )
        )
    };

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
            {new GrabScenarioAction(ItemID.ID_CARD, PointOfInterestId.ID_CARD), new IdCardGiveScenarioNode() }
        };
}

public class IdCardGrabScenarioNodeV2 : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() { new RemoveGrabbableItem(ItemID.ID_CARD_V2, PointOfInterestId.ID_CARD_V2) };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() { new AddGrabbableItem(ItemID.ID_CARD_V2, PointOfInterestId.ID_CARD_V2,
        new AnimatorAction(PlayerAnimatioNamesEnum.PLAYER_ACTION_GRAB_DOWN, new GrabAction(ItemID.ID_CARD_V2, true, null))
       )};

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
          {new GrabScenarioAction(ItemID.ID_CARD_V2, PointOfInterestId.ID_CARD_V2), new IdCardGiveScenarioNodeV2() }
        };
}

public class DumpsterScenarioNode : TimelineNode
{
    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>() {
        { new GrabScenarioAction(ItemID.ID_CARD, PointOfInterestId.DUMBSTER), new IdCardGiveScenarioNode() }
    };

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() {
        new AddGrabbableItem(ItemID.ID_CARD ,PointOfInterestId.DUMBSTER, new CutsceneTimelineAction(CutsceneId.PLAYER_DUMPSTER_GRAB, new GrabAction(ItemID.ID_CARD, false, null)))
    };

    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() { new RemoveGrabbableItem(ItemID.ID_CARD, PointOfInterestId.DUMBSTER) };
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
#endregion

#region DiscussionScenarioNode

public class BouncerKODiscussionNode : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>();

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() {
             new DiscussionTimelineTreeCreationAction(PointOfInterestId.BOUNCER, new DiscussionTree(DiscussionSentencesConstants.Sentenses[DiscussionTreeId.BOUNCER_DISCUSSION_TREE]), new TalkAction(null) )
    };

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
             {new GiveScenarioAction(ItemID.ID_CARD, PointOfInterestId.BOUNCER), new BouncerOKDiscussioNode() }
        };
}

public class BouncerOKDiscussioNode : TimelineNode
{
    public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>();

    public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() {
              new DiscussionTimelineTreeCreationAction(PointOfInterestId.BOUNCER, new DiscussionTree(DiscussionSentencesConstants.Sentenses[DiscussionTreeId.BOUNCER_OK_DISCUSSION]), new TalkAction(null) )
    };

    public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => null;
}
#endregion