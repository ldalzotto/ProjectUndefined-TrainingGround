using CoreGame;

namespace Timelines
{
    public class TimelinesEventManager : GameSingleton<TimelinesEventManager>
    {
        public void OnScenarioActionExecuted(TimeLineAction scenarioAction)
        {
            foreach (var timeline in ATimelinesManager.Get().TimelineManagers)
            {
                timeline.IncrementGraph(scenarioAction);
            }
        }
    }
}