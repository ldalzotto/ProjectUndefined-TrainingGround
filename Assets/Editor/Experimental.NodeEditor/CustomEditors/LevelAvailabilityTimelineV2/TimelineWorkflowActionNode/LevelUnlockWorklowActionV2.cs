using UnityEngine;
using System.Collections;
using NodeGraph;
using System.Collections.Generic;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public class LevelUnlockWorklowActionV2 : NodeProfile
    {
        public LevelUnlockWorklowActionEdgeV2 LevelUnlockWorklowActionEdgeV2;
        public WorkflowActionToNodeEdge WorkflowActionToNodeEdge;
        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.LevelUnlockWorklowActionEdgeV2 = NodeEdgeProfile.CreateNodeEdge<LevelUnlockWorklowActionEdgeV2>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.LevelUnlockWorklowActionEdgeV2 };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.WorkflowActionToNodeEdge = NodeEdgeProfile.CreateNodeEdge<WorkflowActionToNodeEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.WorkflowActionToNodeEdge };
        }
    }

}
