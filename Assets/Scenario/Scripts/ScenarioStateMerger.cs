public class PointOfInterestScenarioStateMerger
{
    public static void AddPointOfInterestScenarioState(ScenarioNode scenarioNode, PointOfInterestManager PointOfInterestManager, ScenarioStateMergerAction scenarioStateMergerAction)
    {
        foreach (var transitionRequirement in scenarioNode.TransitionRequirements)
        {
            var scenarioAction = transitionRequirement.Key;
            var associatedPOI = PointOfInterestManager.GetActivePointOfInterest(scenarioAction.PoiInvolved);

            if (scenarioAction.ActionType == typeof(GiveAction))
            {
                if (scenarioAction.ItemInvolved != ItemID.NONE)
                {
                    if (associatedPOI.PointOfInterestScenarioState.ReceivableItemsComponent == null)
                    {
                        associatedPOI.PointOfInterestScenarioState.ReceivableItemsComponent = new ReceivableItemsComponent();
                    }
                    switch (scenarioStateMergerAction)
                    {
                        case ScenarioStateMergerAction.ADD:
                            associatedPOI.PointOfInterestScenarioState.ReceivableItemsComponent.AddItemID(scenarioAction.ItemInvolved);
                            break;

                        case ScenarioStateMergerAction.DELETE:
                            associatedPOI.PointOfInterestScenarioState.ReceivableItemsComponent.RemoveItemID(scenarioAction.ItemInvolved);
                            break;
                    }
                }
            }
        }
    }
}

public enum ScenarioStateMergerAction
{
    ADD, DELETE
}
