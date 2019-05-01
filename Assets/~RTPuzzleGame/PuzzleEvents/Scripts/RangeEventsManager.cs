using UnityEngine;
using System.Collections;
using System;

namespace RTPuzzle
{
    public class RangeEventsManager : MonoBehaviour
    {

        #region External dependencies
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        #endregion

        public void Init()
        {
            this.GroundEffectsManagerV2 = GameObject.FindObjectOfType<GroundEffectsManagerV2>();
        }

        public void RANGE_EVT_Range_Created(RangeType rangeType)
        {
            this.GroundEffectsManagerV2.OnRangeAdded(rangeType);
        }

        internal void RANGE_EVT_Range_Deleted(RangeType rangeType)
        {
            this.GroundEffectsManagerV2.OnRangeDeleted(rangeType);
        }

    }
}
