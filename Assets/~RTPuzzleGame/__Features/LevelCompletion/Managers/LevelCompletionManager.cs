using UnityEngine;
using System.Collections;
using System;
using CoreGame;

namespace RTPuzzle
{

    public class LevelCompletionManager : GameSingleton<LevelCompletionManager>
    {
        #region External dependencies
        private PuzzleEventsManager ILevelCompletionManagerEventListener;
        private PuzzlePrefabConfiguration PuzzlePrefabConfiguration;
        #endregion
        
        public LevelCompletionManager()
        {
            #region External dependencies
            var PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            this.ILevelCompletionManagerEventListener = PuzzleEventsManager.Get();
            this.PuzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            #endregion
        }

        public void OnLevelCompleted()
        {
            this.ILevelCompletionManagerEventListener.PZ_EVT_LevelCompleted();
        }
    }
}
