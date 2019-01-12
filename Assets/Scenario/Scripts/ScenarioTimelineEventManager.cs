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
            PointOfInterestScenarioStateMerger.MergePointOfInterestScenarioState(scenarioNode, PointOfInterestManager, ScenarioStateMergerAction.DELETE);
        }
    }

    public void OnScenarioNodeStarted(ScenarioNode scenarioNode)
    {
        if (scenarioNode != null)
        {
            PointOfInterestScenarioStateMerger.MergePointOfInterestScenarioState(scenarioNode, PointOfInterestManager, ScenarioStateMergerAction.ADD);
        }
    }

}
