using UnityEngine;

namespace RTPuzzle
{
    public class RangeEventsManager : MonoBehaviour
    {

        #region External dependencies
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        private InRangeEffectManager InRangeEffectManager;
        private ObjectRepelLineVisualFeedbackManager ObjectRepelLineVisualFeedbackManager;
        private RangeTypeObjectContainer RangeTypeObjectContainer;
        #endregion

        public void Init()
        {
            this.GroundEffectsManagerV2 = GameObject.FindObjectOfType<GroundEffectsManagerV2>();
            this.InRangeEffectManager = GameObject.FindObjectOfType<InRangeEffectManager>();
            this.ObjectRepelLineVisualFeedbackManager = GameObject.FindObjectOfType<ObjectRepelLineVisualFeedbackManager>();
            this.RangeTypeObjectContainer = GameObject.FindObjectOfType<RangeTypeObjectContainer>();
        }

        #region Workflow
        public void RANGE_EVT_Range_Created(RangeTypeObject rangeTypeObject)
        {
            this.RangeTypeObjectContainer.AddRange(rangeTypeObject);
            this.GroundEffectsManagerV2.OnRangeAdded(rangeTypeObject);
        }

        public void RANGE_EVT_Range_Destroy(RangeTypeObject rangeTypeObject)
        {
            this.RangeTypeObjectContainer.RemoveRange(rangeTypeObject);
            this.GroundEffectsManagerV2.OnRangeDestroy(rangeTypeObject);
            this.InRangeEffectManager.OnRangeDestroy(rangeTypeObject.RangeType);
            this.ObjectRepelLineVisualFeedbackManager.OnRangeDestroyed(rangeTypeObject.RangeType);
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
