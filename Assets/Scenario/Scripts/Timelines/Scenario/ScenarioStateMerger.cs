﻿public class ReceivableItemScenarioStateMerger
{
    public static void MergeReceivableItemScenarioState(TimelineNode timelineNode, PointOfInterestManager PointOfInterestManager, ScenarioNodeLifecycle scenarioNodeLifecycle)
    {
        foreach (var transitionRequirement in timelineNode.TransitionRequirements)
        {
            var awaitedScenarioAction = transitionRequirement.Key;

            if (awaitedScenarioAction.GetType() == typeof(GiveScenarioAction))
            {
                var giveScenarioAction = (GiveScenarioAction)awaitedScenarioAction;

                if (giveScenarioAction.ItemInvolved != ItemID.NONE)
                {
                    var associatedPOI = PointOfInterestManager.GetActivePointOfInterest(giveScenarioAction.PoiInvolved);
                    if (scenarioNodeLifecycle == ScenarioNodeLifecycle.ON_END)
                    {
                        associatedPOI.OnReceivableItemRemove(giveScenarioAction.ItemInvolved);
                    }
                    else
                    {
                        associatedPOI.OnReceivableItemAdd(giveScenarioAction.ItemInvolved);
                    }

                }

            }
        }
    }
}

public class GrabbableItemScenarioStateMerger
{
    public static void MergeGrabbableItemScenarioState(TimelineNode timelineNode, PointOfInterestManager PointOfInterestManager, ScenarioNodeLifecycle scenarioNodeLifecycle)
    {
        foreach (var transitionRequirement in timelineNode.TransitionRequirements)
        {
            var awaitedScenarioAction = transitionRequirement.Key;

            if (awaitedScenarioAction.GetType() == typeof(GrabScenarioAction))
            {
                var grabScenarioAction = (GrabScenarioAction)awaitedScenarioAction;

                var associatedPOI = PointOfInterestManager.GetActivePointOfInterest(grabScenarioAction.PoiInvolved);
                if (grabScenarioAction.ItemInvolved != ItemID.NONE)
                {
                    if (scenarioNodeLifecycle == ScenarioNodeLifecycle.ON_START)
                    {
                        associatedPOI.OnGrabbableItemAdd(grabScenarioAction.ItemInvolved);
                    }
                    else
                    {
                        associatedPOI.OnGrabbableItemRemove(grabScenarioAction.ItemInvolved);
                    }
                }

            }
        }
    }
}

public enum ScenarioNodeLifecycle
{
    ON_END, ON_START
}