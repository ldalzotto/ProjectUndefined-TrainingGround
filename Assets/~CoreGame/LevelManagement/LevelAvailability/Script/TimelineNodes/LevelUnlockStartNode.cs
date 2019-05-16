using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CoreGame
{
    [System.Serializable]
    public class LevelUnlockStartNode : TimelineNode<LevelAvailabilityManager>
    {
        public override Dictionary<TimeLineAction, TimelineNode<LevelAvailabilityManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<LevelAvailabilityManager>>()
        {
            { new LevelCompletedTimelineAction(LevelZonesID.SEWER_RTP), new LevelUnlockNode() }
        };

        public override List<TimelineNodeWorkflowAction<LevelAvailabilityManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<LevelAvailabilityManager>>();

        public override List<TimelineNodeWorkflowAction<LevelAvailabilityManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<LevelAvailabilityManager>>();
    }

}

