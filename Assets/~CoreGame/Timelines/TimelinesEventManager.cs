using CoreGame;
using UnityEngine;

namespace CoreGame
{
    public class TimelinesEventManager : MonoBehaviour
    {

        public void OnScenarioActionExecuted(TimeLineAction scenarioAction)
        {
            foreach (var timeline in CoreGameSingletonInstances.ATimelinesManager.GetAllTimelines())
            {
                timeline.IncrementGraph(scenarioAction);
            }
        }


    }


}
