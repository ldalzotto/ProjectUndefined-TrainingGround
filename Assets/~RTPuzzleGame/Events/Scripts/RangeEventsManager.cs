using UnityEngine;

namespace RTPuzzle
{
    public class RangeEventsManager : MonoBehaviour, IRangeTypeObjectEventListener, IInRangeColliderTrackerModuleEventListener
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

        #region IRangeTypeObjectEventListener
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

        #region IInRangeColliderTrackerModuleEventListener
        public void RANGE_EVT_InsideRangeTracker(InRangeColliderTrackerModule InRangeColliderTrackerModule, RangeType triggeredRangeType)
        {
            this.InRangeEffectManager.OnInRangeAdd(InRangeColliderTrackerModule, triggeredRangeType);
            this.IObjectRepelLineVisualFeedbackManagerEvent.OnRangeInsideRangeTracker(InRangeColliderTrackerModule, triggeredRangeType);
        }

        public void RANGE_EVT_OutsideRangeTracker(InRangeColliderTrackerModule InRangeColliderTrackerModule, RangeType triggeredRangeType)
        {
            this.InRangeEffectManager.OnInRangeRemove(InRangeColliderTrackerModule, triggeredRangeType);
            this.IObjectRepelLineVisualFeedbackManagerEvent.OnRangeOutsideRangeTracker(InRangeColliderTrackerModule, triggeredRangeType);
        }
        #endregion

    }
}
