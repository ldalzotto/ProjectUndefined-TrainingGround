using NodeGraph;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CoreGame
{
    [System.Serializable]
    public class CutsceneStartNode : NodeProfile
    {
        [SerializeField]
        [FormerlySerializedAs("startEdge")]
        public CutsceneActionConnectionEdge StartEdge;

        public List<ICutsceneNode> GetFirstNodes()
        {
            List<ICutsceneNode> nextNodes = new List<ICutsceneNode>();
            if (StartEdge != null) //when instanciated by test
            {
                foreach (var connectedNode in StartEdge.ConnectedNodeEdges.ConvertAll(e => (CutsceneActionConnectionEdge)e))
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
            this.StartEdge = CutsceneActionConnectionEdge.CreateNodeEdge<CutsceneActionConnectionEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.StartEdge };
        }

        protected override Color NodeColor()
        {
            return Color.green;
        }
#endif
    }
}
