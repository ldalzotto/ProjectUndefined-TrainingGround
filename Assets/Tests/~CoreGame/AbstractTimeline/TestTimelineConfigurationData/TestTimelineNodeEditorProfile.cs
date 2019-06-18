using CoreGame;
using Editor_LevelAvailabilityNodeEditor;
using GameConfigurationID;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TestTimelineNodeEditorProfile", menuName = "Test/CoreGame/AbstractTimeline/TestTimelineNodeEditorProfile", order = 1)]
    public class TestTimelineNodeEditorProfile : TimelineNodeEditorProfile
    {
        public override TimelineID TimelineID => TimelineID.TESTING_TIMELINE;
    }

}
