using CoreGame;
using System;

namespace AdventureGame
{
    [Obsolete("Must be switched to V2")]
    public class ScenarioTimelineManager : AScenarioTimeline
    {
        public override TimelineIDs GetTimelineID()
        {
            return TimelineIDs.SCENARIO_TIMELINE;  
        }
    }
}
