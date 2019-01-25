using System;
using System.Collections.Generic;

public class TestSceneInitialisation : TimelineInitializer
{
    public override List<TimelineNode> InitialNodes => new List<TimelineNode>() { new IdCardGrabScenarioNode(), new IdCardGrabScenarioNodeV2() };
    public override Enum TimelineId => TimelineIDs.SCENARIO_TIMELINE;
}