using UnityEngine;
using System.Collections;
using CoreGame;

namespace RTPuzzle
{
    public class GameOverManager : MonoBehaviour
    {
        #region External Dependencies
        private TimeFlowManager TimeFlowManager;
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        public void Init()
        {
            this.TimeFlowManager = PuzzleGameSingletonInstances.TimeFlowManager;
            this.PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
        }

        private bool onGameOver = false;

        public bool OnGameOver { get => onGameOver; }

        public void Tick(float d)
        {
            if (this.TimeFlowManager.NoMoreTimeAvailable())
            {
                this.onGameOver = true;
                this.PuzzleEventsManager.PZ_EVT_GameOver();
            }
        }
    }

}
