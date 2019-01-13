using UnityEngine;

public class ScenarioTimelineEventManager : MonoBehaviour
{

    #region External Dependencies
    private PointOfInterestManager PointOfInterestManager;
    #endregion

    private void Awake()
    {
        this.PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
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
            DiscussionTreeScenarioStateMerger.MergePointOfInterestScenarioState(scenarioNode, PointOfInterestManager);
        }
    }

}

