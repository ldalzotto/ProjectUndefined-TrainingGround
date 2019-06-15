using UnityEngine;

namespace RTPuzzle
{
    public class RangeEventsManager : MonoBehaviour
    {

        #region External dependencies
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        private InRangeEffectManager InRangeEffectManager;
        private ObjectRepelLineVisualFeedbackManager ObjectRepelLineVisualFeedbackManager;
        #endregion

        public void Init()
        {
            this.GroundEffectsManagerV2 = GameObject.FindObjectOfType<GroundEffectsManagerV2>();
            this.InRangeEffectManager = GameObject.FindObjectOfType<InRangeEffectManager>();
            this.ObjectRepelLineVisualFeedbackManager = GameObject.FindObjectOfType<ObjectRepelLineVisualFeedbackManager>();
        }

        #region Workflow
        public void RANGE_EVT_Range_Created(RangeType rangeType)
        {
            this.GroundEffectsManagerV2.OnRangeAdded(rangeType);
        }

        public void RANGE_EVT_Range_Destroy(RangeType rangeType)
        {
            this.GroundEffectsManagerV2.OnRangeDestroy(rangeType);
            this.InRangeEffectManager.OnRangeDestroy(rangeType);
            this.ObjectRepelLineVisualFeedbackManager.OnRangeDestroyed(rangeType);
        }
        #endregion

        #region Collision
        public void RANGE_EVT_InsideRangeTracker(InRangeColliderTracker InRangeColliderTracker, RangeType rangeType)
        {
            this.InRangeEffectManager.OnInRangeAdd(InRangeColliderTracker, rangeType);
            this.ObjectRepelLineVisualFeedbackManager.OnRangeInsideRangeTracker(InRangeColliderTracker, rangeType);
        }

        public void RANGE_EVT_OutsideRangeTracker(InRangeColliderTracker InRangeColliderTracker, RangeType rangeType)
        {
            this.InRangeEffectManager.OnInRangeRemove(InRangeColliderTracker, rangeType);
            this.ObjectRepelLineVisualFeedbackManager.OnRangeOutsideRangeTracker(InRangeColliderTracker, rangeType);
        }
        #endregion

    }
}
