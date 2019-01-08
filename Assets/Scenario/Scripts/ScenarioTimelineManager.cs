using System;
using UnityEngine;

public class ScenarioTimelineManager : MonoBehaviour
{

    #region External Events
    public void OnScenarioActionExecuted(ScenarioAction scenarioAction)
    {
        UpdateScenarioGraph(scenarioAction);
    }
    #endregion

    private void UpdateScenarioGraph(ScenarioAction executedScenarioAction)
    {
        Debug.Log("Scenario graph update. ItemId : " + executedScenarioAction.ItemInvolved + " POIId : " + executedScenarioAction.PoiInvolved + " Action : " + executedScenarioAction.ActionType.ToString());
    }

}

public class ScenarioAction
{
    private Type actionType;
    private ItemID itemInvolved;
    private PointOfInterestId poiInvolved;

    public ScenarioAction(Type actionType, ItemID itemInvolved, PointOfInterestId poiInvolved)
    {
        this.actionType = actionType;
        this.itemInvolved = itemInvolved;
        this.poiInvolved = poiInvolved;
    }

    public ItemID ItemInvolved { get => itemInvolved; }
    public PointOfInterestId PoiInvolved { get => poiInvolved; }
    public Type ActionType { get => actionType; }
}