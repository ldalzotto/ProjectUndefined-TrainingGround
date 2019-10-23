using Timelines;

namespace LevelManagement
{
    public class LevelAvailabilityTimelineManagerV2 : TimelineNodeManagerV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>
    {
        protected override LevelAvailabilityManager workflowActionPassedDataStruct => LevelAvailabilityManager.Get();

        protected override bool isPersisted => true;
        protected override TimelineID TimelineID => TimelineID.LEVEL_AVAILABILITY_TIMELINE;
    }
}