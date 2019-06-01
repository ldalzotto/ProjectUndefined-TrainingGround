using UnityEngine;
using System.Collections;
using NodeGraph;
using System;
using System.Collections.Generic;
using CoreGame;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public abstract class TimelineWorklowActionEdgeV2<T> : NodeEdgeProfile where T : TimelineNodeWorkflowActionV2Drawable
    {
        public T WorkflowAction;
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>();

        protected override void GUI_Impl(Rect rect)
        {
            this.WorkflowAction.ActionGUI();
        }

        protected override Color EdgeColor()
        {
            return Color.yellow;
        }
    }
}