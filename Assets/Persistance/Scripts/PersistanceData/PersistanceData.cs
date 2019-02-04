using System.Collections.Generic;

public abstract class PersistanceData { }

public class PersistedPOIData : PersistanceData
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