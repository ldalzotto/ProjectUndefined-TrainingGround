using UnityEngine;
using System.Collections;
using Editor_LevelAvailabilityNodeEditor;
using AdventureGame;

namespace Editor_ScenarioNodeEditor
{

    [System.Serializable]
    public class AddDiscussionActionEdge : TimelineWorklowActionEdgeV2<AddDiscussionActionV3>, IContextActionSettable
    {
        public void SetContextAction(AContextAction aContextAction)
        {
            this.WorkflowAction.ContextAction = aContextAction;
        }
    }
}