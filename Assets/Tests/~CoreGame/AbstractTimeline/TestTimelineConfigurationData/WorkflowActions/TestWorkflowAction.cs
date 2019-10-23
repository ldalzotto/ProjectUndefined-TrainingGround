using UnityEngine;
using System.Collections;
using CoreGame;
using Timelines;

namespace Tests
{
    [System.Serializable]
    public class TestWorkflowAction : TimelineNodeWorkflowActionV2<TestTimelineContext, TestTimelineKey>
    {
        public override void ActionGUI()
        {
        }

        public override void Execute(TestTimelineContext workflowActionPassedDataStruct, TimelineNodeV2<TestTimelineContext, TestTimelineKey> timelineNodeRefence)
        {
            workflowActionPassedDataStruct.CallCounter += 1;
        }
    }
}