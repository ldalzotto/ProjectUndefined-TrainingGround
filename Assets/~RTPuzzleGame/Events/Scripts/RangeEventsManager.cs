using UnityEngine;

namespace RTPuzzle
{
    public class RangeEventsManager : MonoBehaviour, IRangeTypeObjectEventListener
    {

        #region External dependencies
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        private IObjectRepelLineVisualFeedbackManagerEvent IObjectRepelLineVisualFeedbackManagerEvent;
        private IRangeTypeObjectContainerEvent IRangeTypeObjectContainerEvent;
        #endregion

        public void Init()
        {
            this.GroundEffectsManagerV2 = PuzzleGameSingletonInstances.GroundEffectsManagerV2;
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
            this.IObjectRepelLineVisualFeedbackManagerEvent.OnRangeDestroyed(rangeTypeObject.RangeType);
        }
        #endregion

    }
}
