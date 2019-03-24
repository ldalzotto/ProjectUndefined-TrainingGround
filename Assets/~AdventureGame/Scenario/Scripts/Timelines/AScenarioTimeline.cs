using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public class AScenarioTimeline : ATimelineNodeManager<GhostsPOIManager>
    {
        protected override GhostsPOIManager workflowActionPassedDataStruct => GameObject.FindObjectOfType<GhostsPOIManager>();
    }
}

