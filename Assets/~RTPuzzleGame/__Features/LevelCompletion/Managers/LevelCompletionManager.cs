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
        private FXContainerManager FXContainerManager;
        private PuzzlePrefabConfiguration PuzzlePrefabConfiguration;
        #endregion
        
        public LevelCompletionManager()
        {
            #region External dependencies
            var PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;

            this.ILevelCompletionManagerEventListener = PuzzleGameSingletonInstances.PuzzleEventsManager;
            this.FXContainerManager = CoreGameSingletonInstances.FXContainerManager;
            this.PuzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            #endregion
        }

        public void OnLevelCompleted()
        {
            this.FXContainerManager.TriggerFX(this.PuzzlePrefabConfiguration.LevelCompletedParticleEffect);
            this.ILevelCompletionManagerEventListener.PZ_EVT_LevelCompleted();
        }
    }
}
