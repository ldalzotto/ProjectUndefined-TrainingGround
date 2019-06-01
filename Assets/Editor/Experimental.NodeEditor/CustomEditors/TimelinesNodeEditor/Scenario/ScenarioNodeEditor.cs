using AdventureGame;
using Editor_LevelAvailabilityNodeEditor;
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
            {"WorkflowActions//AddGrabbableItem", typeof(AddGrabbableItemWorkflowActionNodeProfile) },
            {"WorkflowActions//RemoveGrabbableItem",typeof(RemoveGrabbableItemNodeProfile) },
            {"WorkflowActions//AddItemInteractionAction", typeof(AddItemInteractionActionNodeProfile) },
            {"WorkflowActions//AddPOItoItemInteraction", typeof(AddPOItoItemInteractionNodeProfile) },
            {"WorkflowActions//RemovePOItoItemInteraction", typeof(RemovePOItoItemInteractionNodeProfile) },
            {"WorkflowActions//AddTransitionLevel",typeof(AddTransitionLevelNodeProfile) },
            {"WorkflowActions//AddReceivableItem",typeof(AddReceivableItemNodeProfile) },
            {"WorkflowActions//RemoveReceivableItem",typeof(RemoveReceivableItemNodeProfile) },
            {"WorkflowActions//AddItemGive",typeof(AddItemGiveNodeProfile) }
        };
    }
}