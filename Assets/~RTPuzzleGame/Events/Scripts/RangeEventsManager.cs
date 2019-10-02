using UnityEngine;

namespace RTPuzzle
{
    public class RangeEventsManager : MonoBehaviour, IRangeTypeObjectEventListener
    {

        #region External dependencies
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        #endregion

        public void Init()
        {
            this.GroundEffectsManagerV2 = PuzzleGameSingletonInstances.GroundEffectsManagerV2;
        }

        #region IRangeTypeObjectEventListener
        public void RANGE_EVT_Range_Created(RangeObjectV2 RangeObjectV2)
        {
            RangeObjectV2Manager.Get().ReceiveEvent(new RangeObjectV2ManagerAddRangeEvent { AddedRangeObject = RangeObjectV2 });
            this.GroundEffectsManagerV2.OnRangeAddedV2(RangeObjectV2);
        }

        public void RANGE_EVT_Range_Destroy(RangeObjectV2 RangeObjectV2)
        {
            RangeObjectV2Manager.Get().ReceiveEvent(new RangeObjectV2ManagerRemoveRangeEvent { RemovedRangeObject = RangeObjectV2 });
            this.GroundEffectsManagerV2.OnRangeDestroy(RangeObjectV2);
        }
        #endregion

    }
}
