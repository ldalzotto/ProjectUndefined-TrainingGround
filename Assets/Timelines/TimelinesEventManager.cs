using CoreGame;

namespace Timelines
{
    public class TimelinesEventManager : GameSingleton<TimelinesEventManager>
    {
        public void OnScenarioActionExecuted(TimeLineAction scenarioAction)
        {
            foreach (var timeline in ATimelinesManager.Get().GetAllTimelines())
            {
                timeline.IncrementGraph(scenarioAction);
            }
        }
    }
}