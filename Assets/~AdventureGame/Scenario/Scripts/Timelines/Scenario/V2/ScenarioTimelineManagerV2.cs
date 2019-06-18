using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public class ScenarioTimelineManagerV2 : TimelineNodeManagerV2<GhostsPOIManager, ScenarioTimelineNodeID>
    {
        protected override GhostsPOIManager workflowActionPassedDataStruct => GameObject.FindObjectOfType<GhostsPOIManager>();

        protected override TimelineID TimelineID => TimelineID.SCENARIO_TIMELINE;

        protected override bool isPersisted => true;
    }

}
