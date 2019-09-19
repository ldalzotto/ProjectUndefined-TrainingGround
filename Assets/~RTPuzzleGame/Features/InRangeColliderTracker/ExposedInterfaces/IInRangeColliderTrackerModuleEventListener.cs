using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IInRangeColliderTrackerModuleEventListener 
    {
        void RANGE_EVT_InsideRangeTracker(InRangeColliderTrackerModule InRangeColliderTrackerModule, RangeType triggeredRangeType);
        void RANGE_EVT_OutsideRangeTracker(InRangeColliderTrackerModule InRangeColliderTrackerModule, RangeType triggeredRangeType);
    }
}
