using System;
using System.Collections.Generic;

namespace AdventureGame
{
    public class TestSceneDiscussionTimelineInitialisation : TimelineInitializer
    {
        public override List<TimelineNode> InitialNodes => new List<TimelineNode>() { new BouncerKODiscussionNode() };
        public override Enum TimelineId => TimelineIDs.DISCUSSION_TIMELINE;
    }

}

