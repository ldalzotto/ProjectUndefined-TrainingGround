using CoreGame;
using System;
using System.Collections.Generic;

namespace AdventureGame
{
    public class TestSceneInitialisation : TimelineInitializer<GhostsPOIManager>
    {
        public override List<TimelineNode<GhostsPOIManager>> InitialNodes => new List<TimelineNode<GhostsPOIManager>>() { new IdCardGrabScenarioNode(), new DumpsterScenarioNode(), new CrowbarScenarioNode(), new Sewer_TO_Level1TransitionNode(), new Sewer_TO_SewerRTP_TransitionNode() };
        public override Enum TimelineId => TimelineIDs.SCENARIO_TIMELINE;
    }
}
