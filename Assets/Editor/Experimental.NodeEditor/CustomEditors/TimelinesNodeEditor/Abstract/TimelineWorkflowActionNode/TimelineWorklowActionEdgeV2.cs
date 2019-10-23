using System;
using System.Collections.Generic;
using NodeGraph;
using Timelines;
using UnityEngine;

namespace Editor_LevelAvailabilityNodeEditor
{
    [Serializable]
    public abstract class TimelineWorklowActionEdgeV2<T> : NodeEdgeProfile where T : TimelineNodeWorkflowActionV2Drawable
    {
        public T WorkflowAction;
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>();

        protected override void GUI_Impl(Rect rect, ref NodeEditorProfile nodeEditorProfileRef)
        {
            this.WorkflowAction.ActionGUI();
        }

        protected override Color EdgeColor()
        {
            return Color.yellow;
        }
    }
}