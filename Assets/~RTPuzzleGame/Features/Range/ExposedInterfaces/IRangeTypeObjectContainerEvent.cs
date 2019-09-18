using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IRangeTypeObjectContainerEvent 
    {
        void AddRange(RangeTypeObject rangeTypeObject);
        void RemoveRange(RangeTypeObject rangeTypeObject);
    }

}
