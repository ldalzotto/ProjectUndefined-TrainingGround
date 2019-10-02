using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public static class RangeCollisionTypePropertySetter 
    {
        public static void SetIsAttractiveObject(RangeObjectV2 RangeObjectV2, bool value)
        {
            RangeObjectV2.RangeGameObjectV2.CollisionType.IsRTAttractiveObject = value;
        }
    }

}
