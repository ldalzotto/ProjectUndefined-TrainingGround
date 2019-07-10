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

        #region External Dependencies
        private RangeEventsManager RangeEventsManager;
        #endregion

        public void Init()
        {
            this.RangeEventsManager = GameObject.FindObjectOfType<RangeEventsManager>();
        }

        #region External events
        public void AddRange(RangeTypeObject rangeTypeObject)
        {
            this.rangeTypes[rangeTypeObject.RangeType.RangeTypeID] = rangeTypeObject;
            this.RangeEventsManager.RANGE_EVT_Range_Created(rangeTypeObject);
        }
        internal void RemoveRange(RangeTypeObject rangeTypeObject)
        {
            if (this.rangeTypes.Remove(rangeTypeObject.RangeType.RangeTypeID))
            {
                this.RangeEventsManager.RANGE_EVT_Range_Destroy(rangeTypeObject);
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
