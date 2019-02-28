using System;
using System.Collections.Generic;

namespace AdventureGame
{
    public class TestSceneInitialisation : TimelineInitializer
    {
        public override List<TimelineNode> InitialNodes => new List<TimelineNode>() { new IdCardGrabScenarioNode(), new DumpsterScenarioNode(), new CrowbarScenarioNode(), new Sewer_TO_Level1TransitionNode() };
        public override Enum TimelineId => TimelineIDs.SCENARIO_TIMELINE;
    }
}
