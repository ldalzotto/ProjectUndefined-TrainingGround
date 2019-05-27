using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CoreGame
{
    [System.Serializable]
    public class LevelUnlockNodeV2 : TimelineNodeV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>
    {
        public LevelUnlockNodeV2(Dictionary<TimeLineAction, List<LevelAvailabilityTimelineNodeID>> transitionRequirements, List<TimelineNodeWorkflowActionV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>> onStartNodeAction, List<TimelineNodeWorkflowActionV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>> onExitNodeAction) : base(transitionRequirements, onStartNodeAction, onExitNodeAction)
        {
        }
    }

}
