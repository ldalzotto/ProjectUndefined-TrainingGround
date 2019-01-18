using UnityEngine;

public class ScenarioTimelineEventManager : MonoBehaviour
{

    #region External Dependencies
    private PointOfInterestManager PointOfInterestManager;
    private ScenarioTimelineManager ScenarioTimelineManager;
    private DiscussionTimelineManager DiscussionTimelineManager;
    #endregion

    private void Awake()
    {
        this.PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
        ScenarioTimelineManager = GameObject.FindObjectOfType<ScenarioTimelineManager>();
        DiscussionTimelineManager = GameObject.FindObjectOfType<DiscussionTimelineManager>();
    }

    public void OnScenarioNodeEnded(ScenarioNode scenarioNode)
    {
        if (scenarioNode != null)
        {
            GrabbableItemScenarioStateMerger.MergeGrabbableItemScenarioState(scenarioNode, PointOfInterestManager, ScenarioNodeLifecycle.ON_END);
            ReceivableItemScenarioStateMerger.MergeReceivableItemScenarioState(scenarioNode, PointOfInterestManager, ScenarioNodeLifecycle.ON_END);
        }
    }

    public void OnScenarioNodeStarted(ScenarioNode scenarioNode)
    {
        if (scenarioNode != null)
        {
            GrabbableItemScenarioStateMerger.MergeGrabbableItemScenarioState(scenarioNode, PointOfInterestManager, ScenarioNodeLifecycle.ON_START);
            ReceivableItemScenarioStateMerger.MergeReceivableItemScenarioState(scenarioNode, PointOfInterestManager, ScenarioNodeLifecycle.ON_START);
        }
    }

    public void OnScenarioActionExecuted(ScenarioAction scenarioAction)
    {
        ScenarioTimelineManager.OnScenarioActionExecuted(scenarioAction);
        DiscussionTimelineManager.OnScenarioActionExecuted(scenarioAction);
    }

}

