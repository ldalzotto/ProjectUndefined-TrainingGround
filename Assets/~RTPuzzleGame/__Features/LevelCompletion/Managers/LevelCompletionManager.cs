using CoreGame;

namespace RTPuzzle
{
    public class LevelCompletionManager : GameSingleton<LevelCompletionManager>
    {
        #region External dependencies

        private PuzzleEventsManager ILevelCompletionManagerEventListener;

        #endregion

        public LevelCompletionManager()
        {
            #region External dependencies

            this.ILevelCompletionManagerEventListener = PuzzleEventsManager.Get();

            #endregion
        }

        public void OnLevelCompleted()
        {
            this.ILevelCompletionManagerEventListener.PZ_EVT_LevelCompleted();
        }
    }
}