using CoreGame;
using UnityEngine;
using GameConfigurationID;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class ReplaceDiscussionTreeV2 : TimelineNodeWorkflowActionV2<GhostsPOIManager, DiscussionTimelineNodeID>
    {
        [SerializeField]
        private PointOfInterestId PointOfInterestId;
        [SerializeField]
        private DiscussionTreeId DiscussionTreeId;

        public override void Execute(GhostsPOIManager GhostsPOIManager, TimelineNodeV2<GhostsPOIManager, DiscussionTimelineNodeID> timelineNodeRefence)
        {
            var selectedPOI = GhostsPOIManager.GetGhostPOI(PointOfInterestId);
            var talkAction = new TalkAction(null);
            talkAction.ContextActionWheelNodeConfigurationId = SelectionWheelNodeConfigurationId.TALK_CONTEXT_ACTION_WHEEL_CONFIG;
            if (selectedPOI != null)
            {
                selectedPOI.OnDiscussionTreeAdd(DiscussionTreeId, talkAction);
            }
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.PointOfInterestId = (PointOfInterestId)NodeEditorGUILayout.EnumField("talking POI : ", string.Empty, this.PointOfInterestId);
            this.DiscussionTreeId = (DiscussionTreeId)NodeEditorGUILayout.EnumField("discussion tree : ", string.Empty, this.DiscussionTreeId);
        }
#endif
    }
}