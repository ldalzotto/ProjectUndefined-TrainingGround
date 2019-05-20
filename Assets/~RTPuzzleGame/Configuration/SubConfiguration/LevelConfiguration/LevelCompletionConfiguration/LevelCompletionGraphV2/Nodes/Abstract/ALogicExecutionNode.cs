using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;

namespace RTPuzzle
{
    [System.Serializable]
    public abstract class ALogicExecutionNode : NodeProfile
    {
        [SerializeField]
        public BoolNodeEdge FirstBool;
        [SerializeField]
        public BoolNodeEdge SecondBool;

        [SerializeField]
        public BoolNodeEdge OutputBool;

        public abstract ConditionType ConditionType { get; }

#if UNITY_EDITOR
        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.FirstBool = NodeEdgeProfile.CreateNodeEdge<BoolNodeEdge>(this);
            this.SecondBool = NodeEdgeProfile.CreateNodeEdge<BoolNodeEdge>(this);
            return new List<NodeEdgeProfile>() {
                this.FirstBool,
                this.SecondBool
            };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.OutputBool = NodeEdgeProfile.CreateNodeEdge<BoolNodeEdge>(this);
            return new List<NodeEdgeProfile>() {
                this.OutputBool
            };
        }
#endif

        public void ResolveOutput(ref LevelCompletionConditionResolutionInput conditionGraphResolutionInput)
        {
            this.FirstBool.ComputeValue(ref conditionGraphResolutionInput);
            this.SecondBool.ComputeValue(ref conditionGraphResolutionInput);
            /*
            this.FirstBool.Value = ((BoolNodeEdge)this.FirstBool.BackwardConnectedNodeEdge).Value;
            this.SecondBool.Value = ((BoolNodeEdge)this.SecondBool.BackwardConnectedNodeEdge).Value;
            */
            if (this.ConditionType == ConditionType.AND)
            {
                this.OutputBool.Value = this.FirstBool.GetValue() && this.SecondBool.GetValue();
            }
            else
            {
                this.OutputBool.Value = this.FirstBool.GetValue() || this.SecondBool.GetValue();
            }
        }
    }

    public enum ConditionType
    {
        AND, OR
    }

}
