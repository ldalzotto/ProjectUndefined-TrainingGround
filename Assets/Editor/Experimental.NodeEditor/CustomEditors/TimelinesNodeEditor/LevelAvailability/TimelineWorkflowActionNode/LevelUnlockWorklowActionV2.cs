using System;
using LevelManagement;

namespace Editor_LevelAvailabilityNodeEditor
{
    [Serializable]
    public class LevelUnlockWorklowActionV2 : TimelineWorklowActionTemplatedNodeProfile<LevelAvailabilityLevelCompletedWorkflowActionEdge, LevelUnlockWorkflowActionV2>
    {
    }
}