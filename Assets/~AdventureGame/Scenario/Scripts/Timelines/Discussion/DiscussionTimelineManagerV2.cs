using CoreGame;
using System;

namespace AdventureGame
{
    [Obsolete("Must use V2")]
    public class DiscussionTimelineManagerV2 : AScenarioTimeline
    {
        public override TimelineIDs GetTimelineID()
        {
           return TimelineIDs.DISCUSSION_TIMELINE;
        }
    }

}
