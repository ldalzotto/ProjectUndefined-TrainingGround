using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IInRangeEffectManagerEvent
    {
        void OnInRangeAdd(IInRangeColliderTrackerModuleDataRetriever InRangeColliderTrackerModule, RangeType triggeredRangeType);
        void OnInRangeRemove(IInRangeColliderTrackerModuleDataRetriever InRangeColliderTrackerModule, RangeType triggeredRangeType);
        void OnRangeDestroy(RangeType rangeType);
    }
}
