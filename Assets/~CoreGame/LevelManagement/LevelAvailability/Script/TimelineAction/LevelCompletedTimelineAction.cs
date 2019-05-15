using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public class LevelCompletedTimelineAction : TimeLineAction
    {
        private LevelZonesID completedLevelZone;

        public LevelCompletedTimelineAction(LevelZonesID completedLevelZone)
        {
            this.completedLevelZone = completedLevelZone;
        }

        public override bool Equals(object obj)
        {
            return obj is LevelCompletedTimelineAction action &&
                   completedLevelZone == action.completedLevelZone;
        }

        public override int GetHashCode()
        {
            return -1721677994 + completedLevelZone.GetHashCode();
        }
    }
}

