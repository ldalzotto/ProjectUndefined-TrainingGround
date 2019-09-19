using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTPuzzle;
using NodeGraph;

namespace RTPuzzle
{
    [System.Serializable]
    public class PlayerTargetConditionNode : NodeProfile
    {
        [SerializeField]
        private PlayerTargetConditionEdge PlayerTargetConditionEdge;
        [SerializeField]
        private BoolNodeEdge ResultBool;

#if UNITY_EDITOR
        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.PlayerTargetConditionEdge = NodeEdgeProfile.CreateNodeEdge<PlayerTargetConditionEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.PlayerTargetConditionEdge };
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
            var involvedTargetZoneTriggerCollider = ConditionGraphResolutionInput.InteractiveObjectContainer.TargetZones[this.PlayerTargetConditionEdge.TargetZoneID].ILevelCompletionTriggerModuleDataRetriever.GetTargetZoneTriggerCollider();
            this.ResultBool.Value = (bool)involvedTargetZoneTriggerCollider.bounds.Intersects(ConditionGraphResolutionInput.PlayerManagerDataRetriever.GetPlayerPuzzleLogicRootCollier().bounds);
        }

        public bool GetResult() { return (bool)this.ResultBool.Value; }
    }
}
