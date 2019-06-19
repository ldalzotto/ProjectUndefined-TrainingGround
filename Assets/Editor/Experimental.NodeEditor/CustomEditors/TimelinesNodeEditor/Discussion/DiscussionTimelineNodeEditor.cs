using AdventureGame;
using Editor_LevelAvailabilityNodeEditor;
using Editor_ScenarioNodeEditor;
using GameConfigurationID;
using System;
using System.Collections.Generic;

namespace Editor_DiscussionTimelineNodeEditor
{
    public class DiscussionTimelineNodeEditor : TimelineNodeEditor<DiscussionTimelineInitializerV2, GhostsPOIManager, DiscussionTimelineNodeID>
    {
        protected override Type NodeEditorProfileType => typeof(DiscussionTimelineNodeEditorProfile);

        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>()
        {
             {"TimelineStartNode", typeof(TimelineStartNodeProfile) },
             {"TimelineNodes//DiscussionTimelineNode", typeof(DiscussionTimelineNodeProfile) },
             {"TimelineActions//GiveScenarioAction", typeof(GiveScenarioActionTimelineActionNodeProfile) },
             {"WorkflowActions//AddDiscussionTree", typeof(AddDiscussionTreeWorkflowActionNodeProfile) },
             {"WorkflowActions//RemoveDiscussionTree", typeof(RemoveDiscussionTreeNodeProfile) },
        };
    }

}
