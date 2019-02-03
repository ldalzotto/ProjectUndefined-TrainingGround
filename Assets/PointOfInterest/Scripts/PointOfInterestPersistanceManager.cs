using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestPersistanceManager : MonoBehaviour
{

    #region External Dependencies
    private PointOfInterestManager PointOfInterestManager;
    #endregion

    private PointOfInterestPesistanceDataManager pointOfInterestPesistanceDataManager;
    private bool hasBeenInit;

    internal PointOfInterestPesistanceDataManager PointOfInterestPesistanceDataManager
    {
        get
        {
            if (pointOfInterestPesistanceDataManager == null)
            {
                pointOfInterestPesistanceDataManager = new PointOfInterestPesistanceDataManager();
            }
            return pointOfInterestPesistanceDataManager;
        }
    }

    public void Init()
    {
        PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
        if (!hasBeenInit)
        {
            var p = PointOfInterestPesistanceDataManager;
        }
        hasBeenInit = true;
    }

    #region External Events
    public void OnSavePOI()
    {
        PointOfInterestPesistanceDataManager.OnSavePOI(PointOfInterestManager.GetAllPointOfInterests());
    }
    public bool LoadStateToPOI(ref PointOfInterestType freshPOI)
    {
        return PointOfInterestPesistanceDataManager.LoadStateToPOI(ref freshPOI);
    }
    #endregion

}

class PointOfInterestPesistanceDataManager
{

    private Dictionary<PointOfInterestId, PersistedPOIData> poiPersistedDatas = new Dictionary<PointOfInterestId, PersistedPOIData>();

    public void OnSavePOI(List<PointOfInterestType> allPOI)
    {
        foreach (var poi in allPOI)
        {
            poiPersistedDatas[poi.PointOfInterestId] = new PersistedPOIData(poi.PointOfInterestScenarioState, poi.GetRawContextActions());
        }
    }

    public bool LoadStateToPOI(ref PointOfInterestType freshPOI)
    {
        if (poiPersistedDatas.ContainsKey(freshPOI.PointOfInterestId))
        {
            freshPOI.ReplacePointOfInterestState(poiPersistedDatas[freshPOI.PointOfInterestId].PointOfInterestScenarioState);
            var allContextActions = poiPersistedDatas[freshPOI.PointOfInterestId].ContextActions;
            foreach (var contextActionKV in allContextActions)
            {
                contextActionKV.Value.InitExternalDependencies();
            }
            freshPOI.ReplaceContextActions(allContextActions);
            return true;
        }
        return false;
    }

}

class PersistedPOIData
{
    private PointOfInterestScenarioState pointOfInterestScenarioState;
    private Dictionary<string, AContextAction> contextActions;

    public PersistedPOIData(PointOfInterestScenarioState pointOfInterestScenarioState, Dictionary<string, AContextAction> contextActions)
    {
        this.pointOfInterestScenarioState = pointOfInterestScenarioState;
        this.contextActions = contextActions;
    }

    public PointOfInterestScenarioState PointOfInterestScenarioState { get => pointOfInterestScenarioState; }
    public Dictionary<string, AContextAction> ContextActions { get => contextActions; }
}