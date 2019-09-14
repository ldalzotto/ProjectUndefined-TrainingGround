using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GameConfigurationID;

namespace RTPuzzle
{
    public class RangeTypeObjectContainer : MonoBehaviour
    {
        private Dictionary<RangeTypeID, RangeTypeObject> rangeTypes = new Dictionary<RangeTypeID, RangeTypeObject>();

        #region External events
        public void AddRange(RangeTypeObject rangeTypeObject)
        {
            this.rangeTypes[rangeTypeObject.RangeType.RangeTypeID] = rangeTypeObject;
        }

        public void RemoveRange(RangeTypeObject rangeTypeObject)
        {
            this.rangeTypes.Remove(rangeTypeObject.RangeType.RangeTypeID);
        }
        #endregion

        public void Tick(float d)
        {
            foreach (var rangeType in rangeTypes.Values)
            {
                rangeType.Tick(d);
            }
        }

        public void EndOfFrameTick()
        {
            foreach (var rangeType in rangeTypes.Values)
            {
                rangeType.EndOfFrameTick();
            }
        }
    }

}
