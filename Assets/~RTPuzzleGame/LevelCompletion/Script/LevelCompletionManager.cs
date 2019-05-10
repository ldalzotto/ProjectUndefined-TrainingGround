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
        #endregion

        private LevelConfigurationData currentLevelConfiguration;
        private LevelZonesID currentLevelID;
        private ConditionGraphResolutionInput levelCompletionConditionResolutionInput;

        public void Init(LevelZonesID currentLevelID)
        {
            #region External dependencies
            var PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            var TargetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            this.FXContainerManager = GameObject.FindObjectOfType<FXContainerManager>();
            #endregion

            this.currentLevelConfiguration = PuzzleGameConfigurationManager.LevelConfiguration()[currentLevelID];
            this.currentLevelID = currentLevelID;
            this.levelCompletionConditionResolutionInput = new LevelCompletionConditionResolutionInput(NPCAIManagerContainer, TargetZoneContainer);
        }

        internal void ConditionRecalculationEvaluate()
        {
            Debug.Log(MyLog.Format("Level completion recalculation."));
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
            this.PuzzleEventsManager.PZ_EVT_LevelCompleted(this.currentLevelID);
        }
    }
}
