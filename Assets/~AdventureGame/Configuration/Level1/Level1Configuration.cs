
using CoreGame;
using System.Collections.Generic;

namespace AdventureGame
{
    public interface Level1TimelineNode { }

    #region Level1 Nodes

    #region DiscussionScenarioNode

    public class BouncerKODiscussionNode : TimelineNode<GhostsPOIManager>, Level1TimelineNode
    {
        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>();

        public override List<TimelineNodeWorkflowAction<GhostsPOIManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<GhostsPOIManager>>() {
             new DiscussionTimelineTreeCreationAction(PointOfInterestId.BOUNCER, DiscussionTreeId.BOUNCER_DISCUSSION_TREE, new TalkAction(null) )
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
              new DiscussionTimelineTreeCreationAction(PointOfInterestId.BOUNCER, DiscussionTreeId.BOUNCER_OK_DISCUSSION, new TalkAction(null) )
    };

        public override Dictionary<TimeLineAction, TimelineNode<GhostsPOIManager>> TransitionRequirements => null;
    }
    #endregion

    #endregion

}
