using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using NodeGraph;

namespace RTPuzzle
{
    [System.Serializable]
    public class BoolNodeEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() {
            typeof(BoolNodeEdge)
        };

#if UNITY_EDITOR
        protected override void GUI_Impl(Rect rect)
        {
            if (this.Value == null)
            {
                this.Value = false;
            }
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.Toggle(rect, (bool)this.Value);
            EditorGUI.EndDisabledGroup();
        }

        protected override Color EdgeColor()
        {
            return Color.yellow;
        }
#endif

        public bool GetValue()
        {
            return (bool)this.Value;
        }

        public void ComputeValue(ref LevelCompletionConditionResolutionInput ConditionGraphResolutionInput)
        {
            if (this.BackwardConnectedNodeEdge.NodeProfileRef.GetType() == typeof(AITargetConditionNode))
            {
                ((AITargetConditionNode)this.BackwardConnectedNodeEdge.NodeProfileRef).Resolve(ref ConditionGraphResolutionInput);
            }
            else if (this.BackwardConnectedNodeEdge.NodeProfileRef.GetType().IsSubclassOf(typeof(ALogicExecutionNode)))
            {
                ((ALogicExecutionNode)this.BackwardConnectedNodeEdge.NodeProfileRef).ResolveOutput(ref ConditionGraphResolutionInput);
            }
            this.Value = this.BackwardConnectedNodeEdge.Value;
        }


    }

}
