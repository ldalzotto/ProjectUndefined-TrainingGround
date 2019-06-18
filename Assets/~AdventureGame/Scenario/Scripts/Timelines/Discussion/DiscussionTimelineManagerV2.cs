using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{
    public class DiscussionTimelineManagerV2 : TimelineNodeManagerV2<GhostsPOIManager, DiscussionTimelineNodeID>
    {
        protected override GhostsPOIManager workflowActionPassedDataStruct => GameObject.FindObjectOfType<GhostsPOIManager>();

        protected override TimelineID TimelineID => TimelineID.DISCUSSION_TIMELINE;

        protected override bool isPersisted => true;
    }

    public enum DiscussionTimelineNodeID
    {
        BouncerNoEntry = 0,
        BouncerOK = 1,
        SewerRTP1DoorText = 2
    }

}
