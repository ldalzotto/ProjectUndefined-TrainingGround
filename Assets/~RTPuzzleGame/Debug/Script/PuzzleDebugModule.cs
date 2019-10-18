using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleDebugModule : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool InstantProjectileHit;
        public bool TriggerGameOverEvent;
        public bool TriggerLevelSuccessEvent;

        public bool InfiniteTime;

        [Header("Discussion Test")] public DiscussionTreeId DiscussionTreeID;
        public bool PlayerDiscussionTree;

        #region External Dependencies

        private PuzzleEventsManager PuzzleEventsManager;

        #endregion

        public void Init()
        {
            this.PuzzleEventsManager = PuzzleEventsManager.Get();
            if (InstantProjectileHit)
            {
            }

            if (InfiniteTime)
            {
                TimeFlowManager.Get().CHEAT_SetInfiniteTime();
            }
        }

        public void Tick()
        {
            if (TriggerGameOverEvent)
            {
                this.PuzzleEventsManager.PZ_EVT_GameOver();
                TriggerGameOverEvent = false;
            }

            if (TriggerLevelSuccessEvent)
            {
                this.PuzzleEventsManager.PZ_EVT_LevelCompleted();
                TriggerLevelSuccessEvent = false;
            }
        }
#endif
    }
}