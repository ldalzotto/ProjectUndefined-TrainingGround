using UnityEngine;
using System.Collections;
using System;

namespace RTPuzzle
{

    public class LevelCompletionManager : MonoBehaviour
    {
        #region External dependencies
        private NPCAIManagerContainer NPCAIManagerContainer;
        private TargetZoneContainer TargetZoneContainer;
        private PuzzleEventsManager PuzzleEventsManager;
        private FXContainerManager FXContainerManager;
        #endregion

        private LevelConfigurationData currentLevelConfiguration;

        private LevelZonesID currentLevelID;

        public void Init(LevelZonesID currentLevelID)
        {
            #region External dependencies
            var PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            this.NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            this.TargetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            this.FXContainerManager = GameObject.FindObjectOfType<FXContainerManager>();
            #endregion

            this.currentLevelConfiguration = PuzzleGameConfigurationManager.LevelConfiguration()[currentLevelID];
            this.currentLevelID = currentLevelID;
        }

        internal void ConditionRecalculationEvaluate()
        {
            Debug.Log(MyLog.Format("Level completion recalculation."));
            if (this.currentLevelConfiguration.LevelCompletionInherentData != null && this.currentLevelConfiguration.LevelCompletionInherentData.LevelCompletionAIConditions != null)
            {
                foreach (var levelCompletionCondition in this.currentLevelConfiguration.LevelCompletionInherentData.LevelCompletionAIConditions)
                {
                    var involvedTargetZoneTriggerCollider = this.TargetZoneContainer.GetTargetZone(levelCompletionCondition.TargetZoneID).TargetZoneTriggerType.TargetZoneTriggerCollider;
                    var involvedAI = this.NPCAIManagerContainer.GetNPCAiManager(levelCompletionCondition.aiID).GetCollider();
                    if (involvedTargetZoneTriggerCollider.bounds.Intersects(involvedAI.bounds))
                    {
                        this.OnLevelCompleted();
                    }
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
