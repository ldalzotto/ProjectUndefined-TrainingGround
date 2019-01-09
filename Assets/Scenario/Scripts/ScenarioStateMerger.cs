public class PointOfInterestScenarioStateMerger
{
    public static void MergePointOfInterestScenarioState(ScenarioNode scenarioNode, PointOfInterestManager PointOfInterestManager, ScenarioStateMergerAction scenarioStateMergerAction)
    {
        foreach (var transitionRequirement in scenarioNode.TransitionRequirements)
        {
            var scenarioAction = transitionRequirement.Key;

            if (scenarioAction.GetType() == typeof(GiveScenarioAction))
            {
                var giveScenarioAction = (GiveScenarioAction)scenarioAction;
                var associatedPOI = PointOfInterestManager.GetActivePointOfInterest(giveScenarioAction.PoiInvolved);

                PoiScenarioStateReceivableItemsUpdate(scenarioStateMergerAction, giveScenarioAction.ItemInvolved, associatedPOI);
            }
            else if (scenarioAction.GetType() == typeof(GrabScenarioAction))
            {
                var grabScenarioAction = (GrabScenarioAction)scenarioAction;
                var associatedPOI = PointOfInterestManager.GetActivePointOfInterest(grabScenarioAction.PoiInvolved);

                PoiScenarioStateReceivableItemsUpdate(scenarioStateMergerAction, grabScenarioAction.ItemInvolved, associatedPOI);
            }
        }
    }

    private static void PoiScenarioStateReceivableItemsUpdate(ScenarioStateMergerAction scenarioStateMergerAction, ItemID itemInvolved, PointOfInterestType associatedPOI)
    {
        if (itemInvolved != ItemID.NONE)
        {
            if (associatedPOI.PointOfInterestScenarioState.ReceivableItemsComponent == null)
            {
                associatedPOI.PointOfInterestScenarioState.ReceivableItemsComponent = new ReceivableItemsComponent();
            }
            switch (scenarioStateMergerAction)
            {
                case ScenarioStateMergerAction.ADD:
                    associatedPOI.PointOfInterestScenarioState.ReceivableItemsComponent.AddItemID(itemInvolved);
                    break;

                case ScenarioStateMergerAction.DELETE:
                    associatedPOI.PointOfInterestScenarioState.ReceivableItemsComponent.RemoveItemID(itemInvolved);
                    break;
            }
        }
    }
}

public enum ScenarioStateMergerAction
{
    ADD, DELETE
}
