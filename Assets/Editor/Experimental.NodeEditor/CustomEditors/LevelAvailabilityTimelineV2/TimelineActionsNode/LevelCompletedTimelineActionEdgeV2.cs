using UnityEngine;
using System.Collections;
using NodeGraph;
using System;
using System.Collections.Generic;
using UnityEditor;
using CoreGame;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public class LevelCompletedTimelineActionEdgeV2 : NodeEdgeProfile
    {
        public LevelCompletedTimelineAction LevelCompletedTimelineAction;

        [SerializeField]
        private int selectedTimelineActionIndex;
        
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() {
            typeof(LevelCompletedTimelineActionEdgeV2)
        };

        protected override void GUI_Impl(Rect rect)
        {
            if(this.LevelCompletedTimelineAction == null)
            {
                this.LevelCompletedTimelineAction = new LevelCompletedTimelineAction();
            }
            this.LevelCompletedTimelineAction.NodeGUI();            
        }

        protected override Color EdgeColor()
        {
            return Color.cyan;
        }
    }
}