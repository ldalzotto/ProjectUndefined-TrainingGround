using UnityEngine;
using System.Collections;
using CoreGame;

namespace RTPuzzle
{
    public class GameOverManager : GameSingleton<GameOverManager>
    {
        #region External Dependencies
        private ITimeFlowManagerDataRetrieval ITimeFlowManagerDataRetrieval = TimeFlowManager.Get();
        private IGameOverManagerEventListener IGameOverManagerEventListener = PuzzleEventsManager.Get();
        #endregion
        
        private bool onGameOver = false;

        public bool OnGameOver { get => onGameOver; }

        public void Tick(float d)
        {
            if (this.ITimeFlowManagerDataRetrieval.NoMoreTimeAvailable())
            {
                this.onGameOver = true;
                this.IGameOverManagerEventListener.PZ_EVT_GameOver();
            }
        }
    }

}
