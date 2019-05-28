using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public abstract class AScenarioTimeline : TimelineNodeManager<GhostsPOIManager>
    {
        protected override GhostsPOIManager workflowActionPassedDataStruct => GameObject.FindObjectOfType<GhostsPOIManager>();

        protected override bool isPersisted => false;

    }
}

