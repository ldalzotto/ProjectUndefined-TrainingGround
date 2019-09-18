using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTPuzzle;
using NodeGraph;

namespace RTPuzzle
{
    [System.Serializable]
    public class AITargetConditionNode : NodeProfile
    {
        [SerializeField]
        private AITargetConditionEdge AITargetConditionEdge;
        [SerializeField]
        private BoolNodeEdge ResultBool;

#if UNITY_EDITOR
        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.AITargetConditionEdge = NodeEdgeProfile.CreateNodeEdge<AITargetConditionEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.AITargetConditionEdge };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.ResultBool = NodeEdgeProfile.CreateNodeEdge<BoolNodeEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.ResultBool };
        }

        protected override Vector2 Size()
        {
            return new Vector2(200, 100);
        }

        protected override Color NodeColor()
        {
            return Color.red;
        }
#endif

        public void Resolve(ref LevelCompletionConditionResolutionInput ConditionGraphResolutionInput)
        {
            var involvedTargetZoneTriggerCollider = ConditionGraphResolutionInput.InteractiveObjectContainer.TargetZones[this.AITargetConditionEdge.TargetZoneID].LevelCompletionTriggerModule.GetTargetZoneTriggerCollider();
            var involvedAI = ConditionGraphResolutionInput.NPCAIManagerContainer.GetNPCAiManager(this.AITargetConditionEdge.AiID).GetInteractiveObjectTypeDataRetrieval().GetAILogicColliderModule().GetCollider();
            this.ResultBool.Value = (bool)involvedTargetZoneTriggerCollider.bounds.Intersects(involvedAI.bounds);
        }

        public bool GetResult() { return (bool)this.ResultBool.Value; }
    }
}
