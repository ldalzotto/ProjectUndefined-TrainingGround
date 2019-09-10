using CoreGame;
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

        [Header("Cutscene Test")]
        public PuzzleCutsceneGraph Graph;
        public bool playCutsceneGraph;
        private SequencedActionPlayer cutsceneGraphPlayer;

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        public void Init()
        {
            this.PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            if (InstantProjectileHit)
            {
            }
            if (InfiniteTime)
            {
                PuzzleGameSingletonInstances.TimeFlowManager.CHEAT_SetInfiniteTime();
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

            if (playCutsceneGraph)
            {
                playCutsceneGraph = false;
                this.cutsceneGraphPlayer = new SequencedActionPlayer(this.Graph.GetRootActions(), new PuzzleCutsceneActionInput(PuzzleGameSingletonInstances.InteractiveObjectContainer));
                this.cutsceneGraphPlayer.Play();
            }
            if (this.cutsceneGraphPlayer != null)
            {
                this.cutsceneGraphPlayer.Tick(Time.deltaTime);
            }
        }
#endif
    }


}
