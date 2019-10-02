using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IRangeTypeObjectEventListener 
    {
        void RANGE_EVT_Range_Created(RangeObjectV2 RangeObjectV2);
        void RANGE_EVT_Range_Destroy(RangeObjectV2 RangeObjectV2);
    }
}
