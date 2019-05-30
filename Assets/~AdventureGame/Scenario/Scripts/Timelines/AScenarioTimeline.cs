using CoreGame;
using System;
using UnityEngine;

namespace AdventureGame
{
    [Obsolete("Needs to migrate to AScenarioTimelineV2")]
    public abstract class AScenarioTimeline : TimelineNodeManager<GhostsPOIManager>
    {
        protected override GhostsPOIManager workflowActionPassedDataStruct => GameObject.FindObjectOfType<GhostsPOIManager>();

        protected override bool isPersisted => false;

    }
}

