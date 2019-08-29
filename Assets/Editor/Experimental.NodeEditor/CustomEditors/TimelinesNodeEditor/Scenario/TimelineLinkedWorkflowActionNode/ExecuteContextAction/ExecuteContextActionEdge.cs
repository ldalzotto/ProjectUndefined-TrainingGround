using UnityEngine;
using System.Collections;
using AdventureGame;
using Editor_LevelAvailabilityNodeEditor;

namespace Editor_ScenarioNodeEditor
{
    [System.Serializable]
    public class ExecuteContextActionEdge : TimelineWorklowActionEdgeV2<ExecuteContextActionV3>, IContextActionSettable
    {
        public void SetContextAction(AContextAction aContextAction)
        {
            this.WorkflowAction.ContextAction = aContextAction;
        }
    }
}