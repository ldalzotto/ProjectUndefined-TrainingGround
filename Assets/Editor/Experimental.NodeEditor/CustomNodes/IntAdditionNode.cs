using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Experimental.Editor_NodeEditor
{
    [System.Serializable]
    public class IntAdditionNode : NodeProfile
    {

        public override List<NodeEdgeProfile> InitInputEdges()
        {
            return new List<NodeEdgeProfile>() {
                 NodeEdgeProfile.CreateNodeEdge<IntNodeEdge>(this),
                 NodeEdgeProfile.CreateNodeEdge<IntNodeEdge>(this)
            };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            return new List<NodeEdgeProfile>() {
                 NodeEdgeProfile.CreateNodeEdge<IntNodeEdge>(this)
            };
        }

        protected override Color NodeColor()
        {
            return Color.red;
        }
    }

 

}
