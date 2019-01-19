public class ScenarioTimelineEnterAction : TimelineNodeWorkflowAction
{
    public void Execute(PointOfInterestManager PointOfInterestManager, TimelineNode timelineNodeRefence)
    {
        GrabbableItemScenarioStateMerger.MergeGrabbableItemScenarioState(timelineNodeRefence, PointOfInterestManager, ScenarioNodeLifecycle.ON_START);
        ReceivableItemScenarioStateMerger.MergeReceivableItemScenarioState(timelineNodeRefence, PointOfInterestManager, ScenarioNodeLifecycle.ON_START);
    }
}

public class ScenarioTimelineEndAction : TimelineNodeWorkflowAction
{
    public void Execute(PointOfInterestManager PointOfInterestManager, TimelineNode timelineNodeRefence)
    {
        GrabbableItemScenarioStateMerger.MergeGrabbableItemScenarioState(timelineNodeRefence, PointOfInterestManager, ScenarioNodeLifecycle.ON_END);
        ReceivableItemScenarioStateMerger.MergeReceivableItemScenarioState(timelineNodeRefence, PointOfInterestManager, ScenarioNodeLifecycle.ON_END);
    }
}