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

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        public void Init()
        {
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            if (InstantProjectileHit)
            {
            }
            if (InfiniteTime)
            {
                GameObject.FindObjectOfType<TimeFlowManager>().CHEAT_SetInfiniteTime();
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
