using Editor_LevelAvailabilityNodeEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class TestTimelineNodeEditor : TimelineNodeEditor<TestTimelineInitializer, TestTimelineContext, TestTimelineKey>
    {
        protected override Type NodeEditorProfileType => typeof(TestTimelineNodeEditorProfile);

        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>() {
            {"TimelineActions//TestTimelineAction", typeof(TestTimelineActionNodeProfile) },
            {"TimelineNodes//TestTimelineNode", typeof(TestTimelineNodeProfile) },
            {"WorkflowActions//TestWorkflowAction", typeof(TestWorkflowNodeProfile) },
            {"TimelineStartNode", typeof(TimelineStartNodeProfile) }
        };
    }

    public enum TestTimelineKey
    {
        TestTimeline1,
        TestTimeline2,
        TestTimeline3,
        TestTimeline4,
        TestTimeline5
    }
    public class TestTimelineContext
    {
        public int CallCounter = 0;
    }
}
