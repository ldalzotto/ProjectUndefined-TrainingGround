using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeGraph;

namespace RTPuzzle
{
    public class OutputResultNode : NodeProfile
    {

        private BoolNodeEdge ResultBool;

        public bool Resolve(ref LevelCompletionConditionResolutionInput ConditionGraphResolutionInput)
        {
            this.ResultBool.ComputeValue(ref ConditionGraphResolutionInput);
            return (bool)ResultBool.Value;
        }

#if UNITY_EDITOR
        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.ResultBool = NodeEdgeProfile.CreateNodeEdge<BoolNodeEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() {
                this.ResultBool
            };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            return new List<NodeEdgeProfile>();
        }

        protected override Vector2 Size()
        {
            return new Vector2(150,100);
        }

        protected override Color NodeColor()
        {
            return Color.green;
        }
#endif

    }
}