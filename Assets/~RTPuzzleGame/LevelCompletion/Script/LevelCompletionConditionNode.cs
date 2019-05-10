using UnityEngine;
using System.Collections;
using CoreGame;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    public class LevelCompletionConditionNode : ConditionNode
    {
        public AiID aiID;
        public TargetZoneID targetZoneID;

#if UNITY_EDITOR
        public override void SpecificEditorRender()
        {
            this.aiID = (AiID)EditorGUILayout.EnumPopup(this.aiID);
            this.targetZoneID = (TargetZoneID)EditorGUILayout.EnumPopup(this.targetZoneID);
        }
#endif

        protected override bool ConditionEvaluation(ref ConditionGraphResolutionInput ConditionGraphResolutionInput)
        {
            var levelCompletionConditionResolutionInput = (LevelCompletionConditionResolutionInput)ConditionGraphResolutionInput;
            var involvedTargetZoneTriggerCollider = levelCompletionConditionResolutionInput.TargetZoneContainer.GetTargetZone(this.targetZoneID).TargetZoneTriggerType.TargetZoneTriggerCollider;
            var involvedAI = levelCompletionConditionResolutionInput.NPCAIManagerContainer.GetNPCAiManager(this.aiID).GetCollider();
            return involvedTargetZoneTriggerCollider.bounds.Intersects(involvedAI.bounds);
        }
    }


    public class LevelCompletionConditionResolutionInput : ConditionGraphResolutionInput
    {
        private NPCAIManagerContainer nPCAIManagerContainer;
        private TargetZoneContainer targetZoneContainer;

        public LevelCompletionConditionResolutionInput(NPCAIManagerContainer nPCAIManagerContainer, TargetZoneContainer targetZoneContainer)
        {
            this.nPCAIManagerContainer = nPCAIManagerContainer;
            this.targetZoneContainer = targetZoneContainer;
        }

        public NPCAIManagerContainer NPCAIManagerContainer { get => nPCAIManagerContainer; }
        public TargetZoneContainer TargetZoneContainer { get => targetZoneContainer; }
    }
}
