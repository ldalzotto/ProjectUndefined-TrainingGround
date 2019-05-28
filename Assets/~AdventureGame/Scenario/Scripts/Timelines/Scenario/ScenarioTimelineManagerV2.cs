using CoreGame;

namespace AdventureGame
{
    public class ScenarioTimelineManagerV2 : AScenarioTimeline
    {
        public override TimelineIDs GetTimelineID()
        {
            return TimelineIDs.SCENARIO_TIMELINE;
        }
    }
}
