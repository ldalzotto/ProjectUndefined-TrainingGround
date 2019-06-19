using AdventureGame;
using Editor_LevelAvailabilityNodeEditor;
using GameConfigurationID;
using System;
using System.Collections.Generic;

namespace Editor_ScenarioNodeEditor
{
    public class ScenarioNodeEditor : TimelineNodeEditor<ScenarioTimelineInitializerV2, GhostsPOIManager, ScenarioTimelineNodeID>
    {
        protected override Type NodeEditorProfileType => typeof(ScenarioNodeEditorProfile);

        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>() {
            {"TimelineStartNode", typeof(TimelineStartNodeProfile) },
            {"TimelineNodes//ScenarioNode", typeof(ScenarioNodeProfile) },
            {"TimelineActions//GrabScenarioAction", typeof(GrabScenarioActionTimelineActionNodeProfile) },
            {"TimelineActions//CutsceneTimelineAction", typeof(CutsceneTimelineActionNodeProfile) },
            {"TimelineActions//GiveScenarioAction", typeof(GiveScenarioActionTimelineActionNodeProfile) },
            {"TimelineActions//LevelCompleted", typeof(LevelCompletedTimelineActionNodeProfile) },
            {"TemplatedWorkflowActions//AddGrabbableItem", typeof(AddGrabbableItemWorkflowActionNodeProfile) },
            {"TemplatedWorkflowActions//RemoveGrabbableItem",typeof(RemoveGrabbableItemNodeProfile) },
            {"TemplatedWorkflowActions//AddItemInteractionAction", typeof(AddItemInteractionActionNodeProfile) },
            {"TemplatedWorkflowActions//AddPOItoItemInteraction", typeof(AddPOItoItemInteractionNodeProfile) },
            {"TemplatedWorkflowActions//RemovePOItoItemInteraction", typeof(RemovePOItoItemInteractionNodeProfile) },
            {"TemplatedWorkflowActions//AddTransitionLevel",typeof(AddTransitionLevelNodeProfile) },
            {"TemplatedWorkflowActions//AddReceivableItem",typeof(AddReceivableItemNodeProfile) },
            {"TemplatedWorkflowActions//RemoveReceivableItem",typeof(RemoveReceivableItemNodeProfile) },
            {"TemplatedWorkflowActions//AddItemGive",typeof(AddItemGiveNodeProfile) },
            {"TemplatedWorkflowActions//DisablePOI",typeof(DisablePOINodeProfile) },
            {"TemplatedWorkflowActions//EnablePOI",typeof(EnablePOINodeProfile) },
            {"LinkedWorkflowActions//AddItemContextAction", typeof(AddItemContextActionNodeProfile) },
            {"LinkedWorkflowActions//AddDiscussionContextAction", typeof(AddDiscussionActionNodeProfile) },
            {"LinkedWorkflowActions//RemoveDiscussionContextAction", typeof(RemoveDiscussionActionNodeProfile) },
            {"LinkedWorkflowActions//AddContextAction", typeof(AddContextActionNodeProfile) },
            {"BaseContextAction//AnimatorAction", typeof(AnimatorContextActionNodeProfile) },
            {"BaseContextAction//TalkAction", typeof(TalkContextActionNodeProfile) }
        };
    }
}