using CoreGame;
using NodeGraph;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneStartNode : NodeProfile
    {
        [SerializeField]
        private CutsceneActionConnectionEdge startEdge;

        public SequencedAction GetStartAction()
        {
            return this.GetFirstNode().GetAction();
        }

        public ICutsceneNode GetFirstNode()
        {
            return ((ICutsceneNode)startEdge.ConnectedNodeEdges[0].NodeProfileRef);
        }

#if UNITY_EDITOR
        public override List<NodeEdgeProfile> InitInputEdges()
        {
            return new List<NodeEdgeProfile>();
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.startEdge = CutsceneActionConnectionEdge.CreateNodeEdge<CutsceneActionConnectionEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.startEdge };
        }

        protected override Color NodeColor()
        {
            return Color.green;
        }
#endif
    }
}
