using UnityEngine;
using System.Collections;
using System;

namespace RTPuzzle
{
    public class RangeEventsManager : MonoBehaviour
    {

        #region External dependencies
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        private InRangeEffectManager InRangeEffectManager;
        #endregion

        public void Init()
        {
            this.GroundEffectsManagerV2 = GameObject.FindObjectOfType<GroundEffectsManagerV2>();
            this.InRangeEffectManager = GameObject.FindObjectOfType<InRangeEffectManager>();
        }

        public void RANGE_EVT_Range_Created(RangeType rangeType)
        {
            this.GroundEffectsManagerV2.OnRangeAdded(rangeType);
        }

        internal void RANGE_EVT_Range_Destroy(RangeType rangeType)
        {
            this.GroundEffectsManagerV2.OnRangeDestroy(rangeType);
            this.InRangeEffectManager.OnRangeDestroy(rangeType);
        }

    }
}
