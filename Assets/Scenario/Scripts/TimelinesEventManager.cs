using UnityEngine;

public class TimelinesEventManager : MonoBehaviour
{

    #region External Dependencies
    private PointOfInterestManager PointOfInterestManager;
    private ScenarioTimelineManagerV2 ScenarioTimelineManager;
    private DiscussionTimelineManagerV2 DiscussionTimelineManager;
    #endregion

    private void Awake()
    {
        this.PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
        ScenarioTimelineManager = GameObject.FindObjectOfType<ScenarioTimelineManagerV2>();
        DiscussionTimelineManager = GameObject.FindObjectOfType<DiscussionTimelineManagerV2>();
    }


    public void OnScenarioActionExecuted(ScenarioAction scenarioAction)
    {
        ScenarioTimelineManager.IncrementGraph(scenarioAction);
        DiscussionTimelineManager.IncrementGraph(scenarioAction);
    }


}

