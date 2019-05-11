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
        private ConditionGraphResolutionInput levelCompletionConditionResolutionInput;

        public void Init()
        {
            #region External dependencies
            var PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            var TargetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            this.FXContainerManager = GameObject.FindObjectOfType<FXContainerManager>();
            #endregion

            this.currentLevelConfiguration = PuzzleGameConfigurationManager.LevelConfiguration()[this.LevelManager.GetCurrentLevel()];
            this.levelCompletionConditionResolutionInput = new LevelCompletionConditionResolutionInput(NPCAIManagerContainer, TargetZoneContainer);
        }

        internal void ConditionRecalculationEvaluate()
        {
            if (this.currentLevelConfiguration.LevelCompletionInherentData != null && this.currentLevelConfiguration.LevelCompletionInherentData.LevelCompletionConditionConfiguration != null)
            {
                if (this.currentLevelConfiguration.LevelCompletionInherentData.LevelCompletionConditionConfiguration.ResolveGraph(ref this.levelCompletionConditionResolutionInput))
                {
                    this.OnLevelCompleted();
                }
            }
        }

        private void OnLevelCompleted()
        {
            this.FXContainerManager.TriggerFX(PrefabContainer.Instance.LevelCompletedParticleEffect);
            this.PuzzleEventsManager.PZ_EVT_LevelCompleted(this.LevelManager.GetCurrentLevel());
        }
    }
}
