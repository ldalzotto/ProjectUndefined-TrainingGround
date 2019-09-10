using UnityEngine;
using System.Collections;
using System;
using CoreGame;

namespace RTPuzzle
{

    public class LevelCompletionManager : MonoBehaviour
    {
        #region External dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        private FXContainerManager FXContainerManager;
        private LevelManager LevelManager;
        #endregion

        private LevelConfigurationData currentLevelConfiguration;
        private LevelCompletionConditionResolutionInput levelCompletionConditionResolutionInput;

        public void Init()
        {
            #region External dependencies
            var PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;;
            var NPCAIManagerContainer = PuzzleGameSingletonInstances.AIManagerContainer;
            var InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            var PlayerManagerDataRetriever = PuzzleGameSingletonInstances.PlayerManagerDataRetriever;

            this.LevelManager = CoreGameSingletonInstances.LevelManager;
            this.PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            this.FXContainerManager = CoreGameSingletonInstances.FXContainerManager;
            #endregion

            this.currentLevelConfiguration = PuzzleGameConfigurationManager.LevelConfiguration()[this.LevelManager.GetCurrentLevel()];
            this.levelCompletionConditionResolutionInput = new LevelCompletionConditionResolutionInput(NPCAIManagerContainer, InteractiveObjectContainer, PlayerManagerDataRetriever);
        }

        internal void ConditionRecalculationEvaluate()
        {
            if (this.currentLevelConfiguration.LevelCompletionInherentData != null && this.currentLevelConfiguration.LevelCompletionInherentData.ConditionGraphEditorProfile != null)
            {
                if (this.currentLevelConfiguration.LevelCompletionInherentData.ConditionGraphEditorProfile.Resolve(ref this.levelCompletionConditionResolutionInput))
                {
                    this.OnLevelCompleted();
                }
            }
        }

        private void OnLevelCompleted()
        {
            this.FXContainerManager.TriggerFX(PrefabContainer.Instance.LevelCompletedParticleEffect);
            this.PuzzleEventsManager.PZ_EVT_LevelCompleted();
        }
    }
}
