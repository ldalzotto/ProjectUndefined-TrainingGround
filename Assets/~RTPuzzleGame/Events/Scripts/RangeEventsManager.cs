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
            this.GroundEffectsManagerV2 = PuzzleGameSingletonInstances.GroundEffectsManagerV2;
            this.InRangeEffectManager = PuzzleGameSingletonInstances.InRangeEffectManager;
            this.ObjectRepelLineVisualFeedbackManager = PuzzleGameSingletonInstances.ObjectRepelLineVisualFeedbackManager;
            this.RangeTypeObjectContainer = PuzzleGameSingletonInstances.RangeTypeObjectContainer;
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
        public void RANGE_EVT_InsideRangeTracker(InRangeColliderTrackerModule InRangeColliderTrackerModule, RangeType rangeType)
        {
            this.InRangeEffectManager.OnInRangeAdd(InRangeColliderTrackerModule, rangeType);
            this.ObjectRepelLineVisualFeedbackManager.OnRangeInsideRangeTracker(InRangeColliderTrackerModule, rangeType);
        }

        public void RANGE_EVT_OutsideRangeTracker(InRangeColliderTrackerModule InRangeColliderTrackerModule, RangeType rangeType)
        {
            this.InRangeEffectManager.OnInRangeRemove(InRangeColliderTrackerModule, rangeType);
            this.ObjectRepelLineVisualFeedbackManager.OnRangeOutsideRangeTracker(InRangeColliderTrackerModule, rangeType);
        }
        #endregion

    }
}
