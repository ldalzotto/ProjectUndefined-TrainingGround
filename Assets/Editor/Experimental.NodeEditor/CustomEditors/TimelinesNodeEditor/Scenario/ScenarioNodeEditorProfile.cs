using UnityEngine;
using System.Collections;
using Editor_LevelAvailabilityNodeEditor;
using CoreGame;

namespace Editor_ScenarioNodeEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ScenarioNodeEditorProfile", menuName = "Configuration/AdventureGame/ScenarioConfiguration/ScenarioNodeEditorProfile", order = 1)]

    public class ScenarioNodeEditorProfile : TimelineNodeEditorProfile
    {
        public override TimelineIDs TimelineID => TimelineIDs.SCENARIO_TIMELINE;
    }

}
