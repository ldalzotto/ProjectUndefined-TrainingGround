using UnityEngine;
using System.Collections;
using CoreGame;

namespace RTPuzzle
{
    public class GameOverManager : MonoBehaviour
    {
        #region External Dependencies
        private ITimeFlowManagerDataRetrieval ITimeFlowManagerDataRetrieval;
        private IGameOverManagerEventListener IGameOverManagerEventListener;
        #endregion

        public void Init()
        {
            this.ITimeFlowManagerDataRetrieval = PuzzleGameSingletonInstances.TimeFlowManager;
            this.IGameOverManagerEventListener = PuzzleGameSingletonInstances.PuzzleEventsManager;
        }

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
