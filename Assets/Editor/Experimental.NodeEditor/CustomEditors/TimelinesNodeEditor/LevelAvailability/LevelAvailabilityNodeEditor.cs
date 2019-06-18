using UnityEngine;
using System.Collections;
using Experimental.Editor_NodeEditor;
using System;
using System.Collections.Generic;
using UnityEditor;
using NodeGraph;
using CoreGame;
using GameConfigurationID;

namespace Editor_LevelAvailabilityNodeEditor
{
    public class LevelAvailabilityNodeEditor : TimelineNodeEditor<LevelAvailabilityTimelineInitializerV2, LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>
    {
        protected override Type NodeEditorProfileType => typeof(LevelAvailabilityNodeEditorProfile);

        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>()
        {
            {"TimelineActions//LevelCompleted", typeof(LevelCompletedTimelineActionNodeProfile) },
            {"TimelineNodes//LevelAvailabilityNode", typeof(LevelAvailabilityNodeProfile) },
            {"WorkflowActions//LevelUnlock", typeof(LevelUnlockWorklowActionV2) },
            {"TimelineStartNode", typeof(TimelineStartNodeProfile) }
        };
    }

}
