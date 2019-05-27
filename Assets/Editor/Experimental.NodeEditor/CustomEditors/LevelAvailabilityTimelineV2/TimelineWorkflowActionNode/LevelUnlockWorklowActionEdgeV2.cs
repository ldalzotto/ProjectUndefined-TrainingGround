using UnityEngine;
using System.Collections;
using NodeGraph;
using System;
using System.Collections.Generic;
using CoreGame;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public class LevelUnlockWorklowActionEdgeV2 : NodeEdgeProfile
    {
        public LevelUnlockWorkflowActionV2 LevelUnlockWorkflowAction;
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>();

        protected override void GUI_Impl(Rect rect)
        {
            this.LevelUnlockWorkflowAction.ActionGUI();
        }
    }
}