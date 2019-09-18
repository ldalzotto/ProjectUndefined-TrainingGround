using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GameConfigurationID;

namespace RTPuzzle
{
    public class RangeTypeObjectContainer : MonoBehaviour, IRangeTypeObjectContainerEvent
    {
        private HashSet<RangeTypeObject> rangeTypes = new HashSet<RangeTypeObject>();

        #region IRangeTypeObjectContainerEvent
        public void AddRange(RangeTypeObject rangeTypeObject)
        {
            this.rangeTypes.Add(rangeTypeObject);
        }

        public void RemoveRange(RangeTypeObject rangeTypeObject)
        {
            this.rangeTypes.Remove(rangeTypeObject);
        }
        #endregion

        public void Tick(float d)
        {
            foreach (var rangeType in rangeTypes)
            {
                rangeType.Tick(d);
            }
        }
    }

}
