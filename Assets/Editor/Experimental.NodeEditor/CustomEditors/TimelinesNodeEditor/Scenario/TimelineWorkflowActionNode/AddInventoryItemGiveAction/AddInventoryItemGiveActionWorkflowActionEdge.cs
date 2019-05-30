using UnityEngine;
using System.Collections;
using Editor_LevelAvailabilityNodeEditor;
using AdventureGame;

namespace Editor_ScenarioNodeEditor
{
    [System.Serializable]
    public class AddInventoryItemGiveActionWorkflowActionEdge : TimelineWorklowActionEdgeV2<AddInventoryItemGiveActionV2>
    {
        protected override Color EdgeColor()
        {
            return Color.yellow;
        }
    }
}