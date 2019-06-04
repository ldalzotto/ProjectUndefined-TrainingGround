using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public class ScenarioTimelineManagerV2 : TimelineNodeManagerV2<GhostsPOIManager, ScenarioTimelineNodeID>
    {
        protected override GhostsPOIManager workflowActionPassedDataStruct => GameObject.FindObjectOfType<GhostsPOIManager>();

        protected override TimelineIDs TimelineID => TimelineIDs.SCENARIO_TIMELINE;

        protected override bool isPersisted => true;
    }

    public enum ScenarioTimelineNodeID
    {
        Crowbar_POI = 0,
        Crowbar_Inventory = 1,
        SewerEntrance_POI = 2,
        TransitionToSewer = 3,
        IDCard_Inventory = 4,
        IDCard_Grabbed = 7,
        IDCard_Grabbed_END = 8,
        Dumpster_POI = 5,
        Bouncer_POI = 6,
        Sewer_To_SewerRTP_1 = 9,
        Sewer_To_SewerRTP_2 = 10
    }

}
