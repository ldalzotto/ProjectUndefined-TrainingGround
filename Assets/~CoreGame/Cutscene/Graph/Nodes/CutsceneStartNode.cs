using NodeGraph;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public class CutsceneStartNode : NodeProfile
    {
        [SerializeField]
        private CutsceneActionConnectionEdge startEdge;

        public List<ICutsceneNode> GetFirstNodes()
        {
            List<ICutsceneNode> nextNodes = new List<ICutsceneNode>();
            if (startEdge != null) //when instanciated by test
            {
                foreach (var connectedNode in startEdge.ConnectedNodeEdges.ConvertAll(e => (CutsceneActionConnectionEdge)e))
                {
                    var ICutsceneNode = connectedNode.NodeProfileRef as ICutsceneNode;
                    if (ICutsceneNode != null)
                    {
                        nextNodes.Add(ICutsceneNode);
                    }
                }
            }
            return nextNodes;
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
