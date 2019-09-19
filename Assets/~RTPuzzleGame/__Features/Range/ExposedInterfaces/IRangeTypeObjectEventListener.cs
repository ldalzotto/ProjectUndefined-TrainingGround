using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IRangeTypeObjectEventListener 
    {
        void RANGE_EVT_Range_Created(RangeTypeObject rangeTypeObject);
        void RANGE_EVT_Range_Destroy(RangeTypeObject rangeTypeObject);
    }
}
