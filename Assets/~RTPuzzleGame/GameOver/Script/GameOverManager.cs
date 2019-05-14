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
        private LevelManager LevelManager;
        #endregion

        public void Init()
        {
            this.TimeFlowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
        }

        private bool onGameOver = false;

        public bool OnGameOver { get => onGameOver; }

        public void Tick(float d)
        {
            if (this.TimeFlowManager.NoMoreTimeAvailable())
            {
                this.onGameOver = true;
                this.PuzzleEventsManager.PZ_EVT_GameOver(this.LevelManager.GetCurrentLevel());
            }
        }
    }

}
