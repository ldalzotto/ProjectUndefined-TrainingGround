using System.Collections.Generic;

namespace AdventureGame
{

    public interface DiscussionTimelineModifierAction
    {
        void Execute(PointOfInterestManager PointOfInterestManager);
    }

    public class DiscussionTimelineTreeCreationAction : TimelineNodeWorkflowAction
    {
        private PointOfInterestId PointOfInterestId;
        private DiscussionTree DiscussionTree;
        private AContextAction contextActionToAdd;

        public DiscussionTimelineTreeCreationAction(PointOfInterestId pointOfInterestId, DiscussionTree DiscussionTree, AContextAction contextActionToAdd)
        {
            PointOfInterestId = pointOfInterestId;
            this.DiscussionTree = DiscussionTree;
            this.contextActionToAdd = contextActionToAdd;
            this.contextActionToAdd.ContextActionWheelNodeConfigurationId = SelectionWheelNodeConfigurationId.TALK_CONTEXT_ACTION_WHEEL_CONFIG;
        }

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNode timelineNodeRefence)
        {
            var selectedPOI = GhostsPOIManager.GetGhostPOI(PointOfInterestId);
            if (selectedPOI != null)
            {
                selectedPOI.OnDiscussionTreeAdd(DiscussionTree, contextActionToAdd);
            }
        }
    }

    public class DiscussionTimelineTreeChoiceDeleteAction : TimelineNodeWorkflowAction
    {
        private PointOfInterestId PointOfInterestId;
        private DiscussionChoiceTextId DiscussionIdToDelete;
        private Stack<DiscussionNodeId> nodeIdsWalk;

        public DiscussionTimelineTreeChoiceDeleteAction(PointOfInterestId pointOfInterestId, DiscussionChoiceTextId discussionIdToDelete, Stack<DiscussionNodeId> nodeIdsWalk)
        {
            PointOfInterestId = pointOfInterestId;
            DiscussionIdToDelete = discussionIdToDelete;
            this.nodeIdsWalk = nodeIdsWalk;
        }

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNode timelineNodeRefence)
        {
            var selectedPOI = GhostsPOIManager.GetGhostPOI(PointOfInterestId);
            if (selectedPOI != null)
            {
                selectedPOI.GetAssociatedDiscussionTree().BreakConnectionAtEndOfStack(nodeIdsWalk);
            }
        }
    }
}