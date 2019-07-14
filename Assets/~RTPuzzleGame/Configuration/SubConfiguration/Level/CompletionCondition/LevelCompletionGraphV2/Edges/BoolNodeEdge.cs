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
        protected override void GUI_Impl(Rect rect, ref NodeEditorProfile nodeEditorProfileRef)
        {
            if (this.Value == null)
            {
                this.Value = false;
            }
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.Toggle((bool)this.Value, "");
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
            if (this.BackwardConnectedNodeEdges[0].NodeProfileRef.GetType() == typeof(AITargetConditionNode))
            {
                ((AITargetConditionNode)this.BackwardConnectedNodeEdges[0].NodeProfileRef).Resolve(ref ConditionGraphResolutionInput);
            }
            else if (this.BackwardConnectedNodeEdges[0].NodeProfileRef.GetType().IsSubclassOf(typeof(ALogicExecutionNode)))
            {
                ((ALogicExecutionNode)this.BackwardConnectedNodeEdges[0].NodeProfileRef).ResolveOutput(ref ConditionGraphResolutionInput);
            }
            this.Value = this.BackwardConnectedNodeEdges[0].Value;
        }


    }

}
