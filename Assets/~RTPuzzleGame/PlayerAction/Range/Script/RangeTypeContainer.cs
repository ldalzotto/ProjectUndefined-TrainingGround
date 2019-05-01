using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RTPuzzle
{
    public class RangeTypeContainer : MonoBehaviour
    {
        private Dictionary<RangeTypeID, RangeType> rangeTypes = new Dictionary<RangeTypeID, RangeType>();

        #region External Dependencies
        private RangeEventsManager RangeEventsManager;
        #endregion

        public void Init()
        {
            this.RangeEventsManager = GameObject.FindObjectOfType<RangeEventsManager>();
        }

        #region External events
        public void AddRange(RangeType rangeType)
        {
            this.rangeTypes[rangeType.RangeTypeID] = rangeType;
            this.RangeEventsManager.RANGE_EVT_Range_Created(rangeType);
        }
        internal void RemoveRange(RangeType rangeType)
        {
            if (this.rangeTypes.Remove(rangeType.RangeTypeID))
            {
                this.RangeEventsManager.RANGE_EVT_Range_Deleted(rangeType);
            }
        }
        #endregion

        public void Tick(float d)
        {
            foreach (var rangeType in rangeTypes.Values)
            {
                rangeType.Tick(d);
            }
        }
    }

}
