using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IObjectRepelLineVisualFeedbackManagerEvent
    {
        void OnRangeDestroyed(RangeType rangeType);
        void OnRangeInsideRangeTracker(InRangeColliderTrackerModule InRangeColliderTrackerModule, RangeType rangeType);
        void OnRangeOutsideRangeTracker(InRangeColliderTrackerModule InRangeColliderTrackerModule, RangeType rangeType);
    }
}
