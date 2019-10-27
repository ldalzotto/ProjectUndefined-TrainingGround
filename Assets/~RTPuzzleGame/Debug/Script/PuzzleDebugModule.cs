using LevelManagement;
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
                Instance = FindObjectOfType<PuzzleDebugModule>();
            }

            return Instance;
        }


        public bool InstantProjectileHit;
        public bool TriggerLevelSuccessEvent;


        public void Init()
        {
            if (InstantProjectileHit)
            {
            }
        }

        public void Tick()
        {
            if (TriggerLevelSuccessEvent)
            {
                LevelAvailabilityTimelineEventManager.Get().OnLevelCompleted();
                TriggerLevelSuccessEvent = false;
            }
        }
#endif
    }
}