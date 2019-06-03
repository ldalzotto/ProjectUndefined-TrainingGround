using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public interface DiscussionTimelineModifierAction
    {
        void Execute(PointOfInterestManager PointOfInterestManager);
    }

    [System.Serializable]
    public class DiscussionTimelineTreeCreationAction : TimelineNodeWorkflowAction<GhostsPOIManager>
    {
        [SerializeField]
        private PointOfInterestId PointOfInterestId;
        [SerializeField]
        private DiscussionTreeId DiscussionTreeId;
        [SerializeField]
        private AContextAction contextActionToAdd;
        

        public DiscussionTimelineTreeCreationAction(PointOfInterestId pointOfInterestId, DiscussionTreeId DiscussionTreeId, AContextAction contextActionToAdd)
        {
            PointOfInterestId = pointOfInterestId;
            this.DiscussionTreeId = DiscussionTreeId;
            this.contextActionToAdd = contextActionToAdd;
            this.contextActionToAdd.ContextActionWheelNodeConfigurationId = SelectionWheelNodeConfigurationId.TALK_CONTEXT_ACTION_WHEEL_CONFIG;
        }

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNode<GhostsPOIManager> timelineNodeRefence)
        {
            var selectedPOI = GhostsPOIManager.GetGhostPOI(PointOfInterestId);
            if (selectedPOI != null)
            {
                selectedPOI.OnDiscussionTreeAdd(DiscussionTreeId, contextActionToAdd);
            }
        }
    }
}