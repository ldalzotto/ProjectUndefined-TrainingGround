using UnityEngine;
using System.Collections;
using NodeGraph;
using CoreGame;
using System.Collections.Generic;
using System;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public abstract class TimelineActionEdgeProfile<T> : NodeEdgeProfile where T : TimeLineAction
    {
        public T TimelineAction;

        [SerializeField]
        private int selectedTimelineActionIndex;

        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() {
            typeof(T)
        };

        protected override void GUI_Impl(Rect rect, ref NodeEditorProfile nodeEditorProfileRef)
        {
            if (this.TimelineAction == null)
            {
                this.TimelineAction = Activator.CreateInstance<T>();
            }
            this.TimelineAction.NodeGUI();
        }

        protected override Color EdgeColor()
        {
            return Color.cyan;
        }
    }
}