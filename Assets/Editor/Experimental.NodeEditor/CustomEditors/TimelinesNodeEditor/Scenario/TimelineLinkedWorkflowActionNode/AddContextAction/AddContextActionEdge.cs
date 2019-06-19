using UnityEngine;
using System.Collections;
using Editor_LevelAvailabilityNodeEditor;
using AdventureGame;

namespace Editor_ScenarioNodeEditor
{

    [System.Serializable]
    public class AddContextActionEdge : TimelineWorklowActionEdgeV2<AddContextActionV3>, IContextActionSettable
    {
        public void SetContextAction(AContextAction aContextAction)
        {
            this.WorkflowAction.ContextAction = aContextAction;
        }
    }
}