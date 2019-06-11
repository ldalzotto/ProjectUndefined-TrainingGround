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

        private void Start()
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

        private void Update()
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
