using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace CoreGame
{
    public class LevelAvailabilityTimelineInitializer : TimelineInitializer<LevelAvailabilityManager>
    {
        public override List<TimelineNode<LevelAvailabilityManager>> InitialNodes => new List<TimelineNode<LevelAvailabilityManager>>() { new LevelUnlockStartNode() };

        public override Enum TimelineId => TimelineIDs.LEVEL_AVAILABILITY_TIMELINE;
    }


}
