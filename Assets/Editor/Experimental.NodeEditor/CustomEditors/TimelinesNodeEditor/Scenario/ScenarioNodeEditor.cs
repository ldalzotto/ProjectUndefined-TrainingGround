﻿using UnityEngine;
using System.Collections;
using Experimental.Editor_NodeEditor;
using System;
using System.Collections.Generic;
using Editor_LevelAvailabilityNodeEditor;
using AdventureGame;

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
            {"WorkflowActions//AddGrabbableItem", typeof(AddGrabbableItemWorkflowActionNodeProfile) },
            {"WorkflowActions//RemoveGrabbableItem",typeof(RemoveGrabbableItemNodeProfile) },
            {"WorkflowActions//AddInventoryItemGiveAction", typeof(AddInventoryItemGiveActionWorkflowActionNodeProfile) },
            {"WorkflowActions//AddPOIInteractableItem", typeof(AddPOIInteractableItemNodeProfile) },
            {"WorkflowActions//RemovePOIInteractableItem", typeof(RemovePOIInteractableItemNodeProfile) },
            {"WorkflowActions//AddTransitionLevel",typeof(AddTransitionLevelNodeProfile) },
            {"WorkflowActions//AddReceivableItem",typeof(AddReceivableItemNodeProfile) },
            {"WorkflowActions//RemoveReceivableItem",typeof(RemoveReceivableItemNodeProfile) }
        };
    }
}