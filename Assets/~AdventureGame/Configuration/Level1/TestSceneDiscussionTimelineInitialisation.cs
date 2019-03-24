using CoreGame;
using System;
using System.Collections.Generic;

namespace AdventureGame
{
    public class TestSceneDiscussionTimelineInitialisation : TimelineInitializer<GhostsPOIManager>
    {
        public override List<TimelineNode<GhostsPOIManager>> InitialNodes => new List<TimelineNode<GhostsPOIManager>>() { new BouncerKODiscussionNode() };
        public override Enum TimelineId => TimelineIDs.DISCUSSION_TIMELINE;
    }

}

