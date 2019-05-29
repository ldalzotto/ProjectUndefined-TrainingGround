
using CoreGame;
using System.Collections.Generic;

namespace AdventureGame
{

    public interface Level1TimelineNode { }
    public interface Level1_SewerTimelineNode { }

    #region Level1 Nodes

    #region ScenarioNode
    public class CrowbarScenarioNode : TimelineNode<GhostsPOIManager>, Level1TimelineNode
    {
        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>>() {
        { new GrabScenarioAction(ItemID.CROWBAR, PointOfInterestId.CROWBAR), new SewerEntranceScenarioNode()}
    };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>()
    {
        new AddGrabbableItem(ItemID.CROWBAR, PointOfInterestId.CROWBAR, new AnimatorAction(PlayerAnimatioNamesEnum.PLAYER_ACTION_GRAB_DOWN,  new GrabAction(ItemID.CROWBAR, true, null)))
    };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>();
    }

    public class SewerEntranceScenarioNode : TimelineNode<GhostsPOIManager>, Level1TimelineNode
    {
        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>>() {
        {new CutsceneTimelineScenarioAction(CutsceneId.PLAYER_OPEN_SEWER, PointOfInterestId.SEWER_ENTRANCE), new Level1_TO_SewerTransitionNode() }
    };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>()
    {
        new AddPOIInteractableItem(ItemID.CROWBAR, PointOfInterestId.SEWER_ENTRANCE)
    };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() {
        new RemovePOIInteractableItem(ItemID.CROWBAR, PointOfInterestId.SEWER_ENTRANCE)
    };
    }

    public class Level1_TO_SewerTransitionNode : TimelineNode<GhostsPOIManager>, Level1TimelineNode
    {
        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>>()
        {
        };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() {
        new AddTransitionLevel(LevelZonesID.SEWER, PointOfInterestId.SEWER_ENTRANCE, new LevelZoneTransitionAction(LevelZonesID.SEWER))
    };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>();
    }

    public class IdCardGrabScenarioNode : TimelineNode<GhostsPOIManager>, Level1TimelineNode
    {
        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() {
        new RemoveGrabbableItem(ItemID.ID_CARD, PointOfInterestId.ID_CARD)
    };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() {
        new AddGrabbableItem(ItemID.ID_CARD, PointOfInterestId.ID_CARD,
            new AnimatorAction( PlayerAnimatioNamesEnum.PLAYER_ACTION_GRAB_DOWN,
                new GrabAction(ItemID.ID_CARD, true,
                   new DummyContextAction(null)
                )
            )
        )
    };

        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>>()
        {
            {new GrabScenarioAction(ItemID.ID_CARD, PointOfInterestId.ID_CARD), new IdCardGiveScenarioNode() }
        };
    }

    public class DumpsterScenarioNode : TimelineNode<GhostsPOIManager>, Level1TimelineNode
    {
        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>>() {
        { new GrabScenarioAction(ItemID.ID_CARD, PointOfInterestId.DUMBSTER), new IdCardGiveScenarioNode() }
    };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() {
        new AddGrabbableItem(ItemID.ID_CARD ,PointOfInterestId.DUMBSTER, new CutsceneTimelineAction(CutsceneId.PLAYER_DUMPSTER_GRAB, new GrabAction(ItemID.ID_CARD, false, null)))
    };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() { new RemoveGrabbableItem(ItemID.ID_CARD, PointOfInterestId.DUMBSTER) };
    }

    public class IdCardGiveScenarioNode : TimelineNode<GhostsPOIManager>, Level1TimelineNode
    {
        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() { new RemoveReceivableItem(ItemID.ID_CARD, PointOfInterestId.BOUNCER) };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() { new AddReceivableItem(ItemID.ID_CARD, PointOfInterestId.BOUNCER) };

        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>>()
        {
           {new GiveScenarioAction(ItemID.ID_CARD, PointOfInterestId.BOUNCER), null }
        };
    }

    public class CrowBar_InventoryScenarioNode : TimelineNode<GhostsPOIManager>, Level1TimelineNode
    {
        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>>();

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>()
        {
            new AddInventoryItemGiveAction(ItemID.CROWBAR, PointOfInterestId.CROWBAR_INVENTORY, new InteractAction(ItemID.CROWBAR, new CutsceneTimelineAction(CutsceneId.PLAYER_OPEN_SEWER, null)) )
        };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>();
    }

    public class IdCard_InventoryScenarioNode : TimelineNode<GhostsPOIManager>, Level1TimelineNode
    {
        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>>();

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>()
        {
            new AddInventoryItemGiveAction(ItemID.ID_CARD, PointOfInterestId.ID_CARD_INVENTORY,new GiveAction(ItemID.ID_CARD, null))
        };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>();
    }

    #endregion

    #region DiscussionScenarioNode

    public class BouncerKODiscussionNode : TimelineNode<GhostsPOIManager>, Level1TimelineNode
    {
        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>();

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() {
             new DiscussionTimelineTreeCreationAction(PointOfInterestId.BOUNCER, new DiscussionTree(DiscussionSentencesConstants.Sentenses[DiscussionTreeId.BOUNCER_DISCUSSION_TREE]), new TalkAction(null) )
    };

        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>>()
        {
             {new GiveScenarioAction(ItemID.ID_CARD, PointOfInterestId.BOUNCER), new BouncerOKDiscussioNode() }
        };
    }

    public class BouncerOKDiscussioNode : TimelineNode<GhostsPOIManager>, Level1TimelineNode
    {
        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>();

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() {
              new DiscussionTimelineTreeCreationAction(PointOfInterestId.BOUNCER, new DiscussionTree(DiscussionSentencesConstants.Sentenses[DiscussionTreeId.BOUNCER_OK_DISCUSSION]), new TalkAction(null) )
    };

        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => null;
    }
    #endregion

    #endregion

    #region Sewer Nodes
    public class Sewer_TO_Level1TransitionNode : TimelineNode<GhostsPOIManager>, Level1_SewerTimelineNode
    {
        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>>();

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() {
        new AddTransitionLevel(LevelZonesID.LEVEL1, PointOfInterestId.SEWER_EXIT, new LevelZoneTransitionAction(LevelZonesID.LEVEL1))
    };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>();
    }

    public class Sewer_TO_SewerRTP_TransitionNode : TimelineNode<GhostsPOIManager>, Level1_SewerTimelineNode
    {
        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>>();

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>()
        {
            new AddTransitionLevel(LevelZonesID.SEWER_ADVENTURE, PointOfInterestId.SEWER_TO_PUZZLE, new LevelZoneTransitionAction(LevelZonesID.SEWER_RTP))
        };

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>();
    }
    #endregion
}
