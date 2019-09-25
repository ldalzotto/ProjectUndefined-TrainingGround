using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IObjectRepelLineVisualFeedbackManagerEvent
    {
        void OnRangeDestroyed(RangeType rangeType);
        void OnRangeInsideRangeTracker(InRangeVisualFeedbackModule InRangeColliderTrackerModule, RangeType rangeType);
        void OnRangeOutsideRangeTracker(InRangeVisualFeedbackModule InRangeColliderTrackerModule, RangeType rangeType);
    }
}
