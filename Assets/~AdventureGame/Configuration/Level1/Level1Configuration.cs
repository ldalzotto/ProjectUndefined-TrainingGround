
using System.Collections.Generic;

namespace AdventureGame
{

    public interface Level1TimelineNode { }
    public interface Level1_SewerTimelineNode { }

    #region Level1 Nodes

    #region ScenarioNode
    public class CrowbarScenarioNode : TimelineNode, Level1TimelineNode
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

    public class SewerEntranceScenarioNode : TimelineNode, Level1TimelineNode
    {
        public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>() {
        {new CutsceneTimelineScenarioAction(CutsceneId.PLAYER_OPEN_SEWER, PointOfInterestId.SEWER_ENTRANCE), new Level1_TO_SewerTransitionNode() }
    };

        public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>()
    {
        new AddPOIInteractableItem(ItemID.CROWBAR, PointOfInterestId.SEWER_ENTRANCE)
    };

        public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() {
        new RemovePOIInteractableItem(ItemID.CROWBAR, PointOfInterestId.SEWER_ENTRANCE)
    };
    }

    public class Level1_TO_SewerTransitionNode : TimelineNode, Level1TimelineNode
    {
        public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
        };

        public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() {
        new AddTransitionLevel(LevelZonesID.SEWER, PointOfInterestId.SEWER_ENTRANCE, new LevelZoneTransitionAction(LevelZonesID.SEWER))
    };

        public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>();
    }

    public class IdCardGrabScenarioNode : TimelineNode, Level1TimelineNode
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

    public class DumpsterScenarioNode : TimelineNode, Level1TimelineNode
    {
        public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>() {
        { new GrabScenarioAction(ItemID.ID_CARD, PointOfInterestId.DUMBSTER), new IdCardGiveScenarioNode() }
    };

        public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() {
        new AddGrabbableItem(ItemID.ID_CARD ,PointOfInterestId.DUMBSTER, new CutsceneTimelineAction(CutsceneId.PLAYER_DUMPSTER_GRAB, new GrabAction(ItemID.ID_CARD, false, null)))
    };

        public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() { new RemoveGrabbableItem(ItemID.ID_CARD, PointOfInterestId.DUMBSTER) };
    }

    public class IdCardGiveScenarioNode : TimelineNode, Level1TimelineNode
    {
        public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>() { new RemoveReceivableItem(ItemID.ID_CARD, PointOfInterestId.BOUNCER) };

        public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() { new AddReceivableItem(ItemID.ID_CARD, PointOfInterestId.BOUNCER) };

        public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>()
        {
           {new GiveScenarioAction(ItemID.ID_CARD, PointOfInterestId.BOUNCER), null }
        };
    }

    #endregion

    #region DiscussionScenarioNode

    public class BouncerKODiscussionNode : TimelineNode, Level1TimelineNode
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

    public class BouncerOKDiscussioNode : TimelineNode, Level1TimelineNode
    {
        public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>();

        public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() {
              new DiscussionTimelineTreeCreationAction(PointOfInterestId.BOUNCER, new DiscussionTree(DiscussionSentencesConstants.Sentenses[DiscussionTreeId.BOUNCER_OK_DISCUSSION]), new TalkAction(null) )
    };

        public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => null;
    }
    #endregion

    #endregion

    #region Sewer Nodes
    public class Sewer_TO_Level1TransitionNode : TimelineNode, Level1_SewerTimelineNode
    {
        public override Dictionary<ScenarioAction, TimelineNode> TransitionRequirements => new Dictionary<ScenarioAction, TimelineNode>();

        public override List<TimelineNodeWorkflowAction> OnStartNodeAction => new List<TimelineNodeWorkflowAction>() {
        new AddTransitionLevel(LevelZonesID.LEVEL1, PointOfInterestId.SEWER_EXIT, new LevelZoneTransitionAction(LevelZonesID.LEVEL1))
    };

        public override List<TimelineNodeWorkflowAction> OnExitNodeAction => new List<TimelineNodeWorkflowAction>();
    }
    #endregion
}
