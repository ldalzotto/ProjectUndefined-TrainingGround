using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public class LevelUnlockNode : TimelineNode<LevelAvailabilityManager>
    {
        public override Dictionary<TimeLineAction, TimelineNode<LevelAvailabilityManager>> TransitionRequirements => new Dictionary<TimeLineAction, TimelineNode<LevelAvailabilityManager>>();

        public override List<TimelineNodeWorkflowAction<LevelAvailabilityManager>> OnStartNodeAction => new List<TimelineNodeWorkflowAction<LevelAvailabilityManager>>()
        {
            new LevelUnlockWorkflowAction(LevelZoneChunkID.SEWER_RTP_2)
        };

        public override List<TimelineNodeWorkflowAction<LevelAvailabilityManager>> OnExitNodeAction => new List<TimelineNodeWorkflowAction<LevelAvailabilityManager>>();
    }

}
