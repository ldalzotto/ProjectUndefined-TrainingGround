using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleDebugModule : MonoBehaviour
    {
#if UNITY_EDITOR
        private static PuzzleDebugModule Instance;

        public static PuzzleDebugModule Get()
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<PuzzleDebugModule>();
            }

            return Instance;
        }


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