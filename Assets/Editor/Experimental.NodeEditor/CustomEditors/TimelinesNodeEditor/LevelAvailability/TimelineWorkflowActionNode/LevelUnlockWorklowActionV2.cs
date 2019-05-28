using UnityEngine;
using System.Collections;
using NodeGraph;
using System.Collections.Generic;
using CoreGame;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public class LevelUnlockWorklowActionV2 : TimelineWorklowActionNodeProfile<LevelAvailabilityLevelCompletedWorkflowActionEdge, LevelUnlockWorkflowActionV2>
    {
    }

}