using UnityEngine;

namespace RTPuzzle
{
    public class RangeEventsManager : MonoBehaviour, IRangeTypeObjectEventListener
    {

        #region External dependencies
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        private InRangeEffectManager InRangeEffectManager;
        private IObjectRepelLineVisualFeedbackManagerEvent IObjectRepelLineVisualFeedbackManagerEvent;
        private IRangeTypeObjectContainerEvent IRangeTypeObjectContainerEvent;
        #endregion

        public void Init()
        {
            this.GroundEffectsManagerV2 = PuzzleGameSingletonInstances.GroundEffectsManagerV2;
            this.InRangeEffectManager = PuzzleGameSingletonInstances.InRangeEffectManager;
            this.IObjectRepelLineVisualFeedbackManagerEvent = PuzzleGameSingletonInstances.ObjectRepelLineVisualFeedbackManager;
            this.IRangeTypeObjectContainerEvent = PuzzleGameSingletonInstances.RangeTypeObjectContainer;
        }

        #region Workflow
        public void RANGE_EVT_Range_Created(RangeTypeObject rangeTypeObject)
        {
            this.IRangeTypeObjectContainerEvent.AddRange(rangeTypeObject);
            this.GroundEffectsManagerV2.OnRangeAdded(rangeTypeObject);
        }

        public void RANGE_EVT_Range_Destroy(RangeTypeObject rangeTypeObject)
        {
            this.IRangeTypeObjectContainerEvent.RemoveRange(rangeTypeObject);
            this.GroundEffectsManagerV2.OnRangeDestroy(rangeTypeObject);
            this.InRangeEffectManager.OnRangeDestroy(rangeTypeObject.RangeType);
            this.IObjectRepelLineVisualFeedbackManagerEvent.OnRangeDestroyed(rangeTypeObject.RangeType);
        }
        #endregion

        #region Collision
        public void RANGE_EVT_InsideRangeTracker(InRangeColliderTrackerModule InRangeColliderTrackerModule, RangeType rangeType)
        {
            this.InRangeEffectManager.OnInRangeAdd(InRangeColliderTrackerModule, rangeType);
            this.IObjectRepelLineVisualFeedbackManagerEvent.OnRangeInsideRangeTracker(InRangeColliderTrackerModule, rangeType);
        }

        public void RANGE_EVT_OutsideRangeTracker(InRangeColliderTrackerModule InRangeColliderTrackerModule, RangeType rangeType)
        {
            this.InRangeEffectManager.OnInRangeRemove(InRangeColliderTrackerModule, rangeType);
            this.IObjectRepelLineVisualFeedbackManagerEvent.OnRangeOutsideRangeTracker(InRangeColliderTrackerModule, rangeType);
        }
        #endregion

    }
}
