using System;
using Editor_LevelAvailabilityNodeEditor;
using Timelines;
using UnityEngine;

namespace Tests
{
    [Serializable]
    [CreateAssetMenu(fileName = "TestTimelineNodeEditorProfile", menuName = "Test/CoreGame/AbstractTimeline/TestTimelineNodeEditorProfile", order = 1)]
    public class TestTimelineNodeEditorProfile : TimelineNodeEditorProfile
    {
        public override TimelineID TimelineID => TimelineID.TESTING_TIMELINE;
    }
}