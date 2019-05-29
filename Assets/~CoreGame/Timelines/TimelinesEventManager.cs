using CoreGame;
using UnityEngine;

namespace CoreGame
{
    public class TimelinesEventManager : MonoBehaviour
    {

        private ATimelinesManager ATimelinesManager;

        public void Init()
        {
            this.ATimelinesManager = GameObject.FindObjectOfType<ATimelinesManager>();
        }

        public void OnScenarioActionExecuted(TimeLineAction scenarioAction)
        {
            foreach (var timeline in this.ATimelinesManager.GetAllTimelines())
            {
                timeline.IncrementGraph(scenarioAction);
            }
        }


    }


}
