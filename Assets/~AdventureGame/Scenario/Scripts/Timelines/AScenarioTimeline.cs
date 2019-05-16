using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public class AScenarioTimeline : TimelineNodeManager<GhostsPOIManager>
    {
        protected override GhostsPOIManager workflowActionPassedDataStruct => GameObject.FindObjectOfType<GhostsPOIManager>();

        protected override bool isPersisted => false;
    }
}

