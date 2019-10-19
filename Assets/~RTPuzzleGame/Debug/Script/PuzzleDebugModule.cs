using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleDebugModule : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool InstantProjectileHit;
        public bool TriggerLevelSuccessEvent;

        #region External Dependencies

        private PuzzleEventsManager PuzzleEventsManager;

        #endregion

        public void Init()
        {
            PuzzleEventsManager = PuzzleEventsManager.Get();
            if (InstantProjectileHit)
            {
            }
        }

        public void Tick()
        {
            if (TriggerLevelSuccessEvent)
            {
                PuzzleEventsManager.PZ_EVT_LevelCompleted();
                TriggerLevelSuccessEvent = false;
            }
        }
#endif
    }
}