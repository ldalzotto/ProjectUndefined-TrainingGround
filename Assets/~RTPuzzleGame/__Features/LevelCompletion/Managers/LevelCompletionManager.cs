﻿using UnityEngine;
using System.Collections;
using System;
using CoreGame;

namespace RTPuzzle
{

    public class LevelCompletionManager : MonoBehaviour, ILevelCompletionManagerEvent
    {
        #region External dependencies
        private ILevelCompletionManagerEventListener ILevelCompletionManagerEventListener;
        private FXContainerManager FXContainerManager;
        private LevelManager LevelManager;
        private PuzzlePrefabConfiguration PuzzlePrefabConfiguration;
        #endregion

        private LevelConfigurationData currentLevelConfiguration;

        public void Init()
        {
            #region External dependencies
            var PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;;
            var NPCAIManagerContainer = PuzzleGameSingletonInstances.AIManagerContainer;
            var InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;

            this.LevelManager = CoreGameSingletonInstances.LevelManager;
            this.ILevelCompletionManagerEventListener = PuzzleGameSingletonInstances.PuzzleEventsManager;
            this.FXContainerManager = CoreGameSingletonInstances.FXContainerManager;
            this.PuzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            #endregion

            this.currentLevelConfiguration = PuzzleGameConfigurationManager.LevelConfiguration()[this.LevelManager.GetCurrentLevel()];
        }

        public void ConditionRecalculationEvaluate()
        {
            
        }

        private void OnLevelCompleted()
        {
            this.FXContainerManager.TriggerFX(this.PuzzlePrefabConfiguration.LevelCompletedParticleEffect);
            this.ILevelCompletionManagerEventListener.PZ_EVT_LevelCompleted();
        }
    }
}
