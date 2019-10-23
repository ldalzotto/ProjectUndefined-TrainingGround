using UnityEngine;
using System.Collections;
using CoreGame;
using Timelines;

namespace Tests
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TestTimelineInitializer", menuName = "Test/CoreGame/AbstractTimeline/TestTimelineInitializer", order = 1)]

    public class TestTimelineInitializer : TimelineInitializerV2<TestTimelineContext, TestTimelineKey>
    {
    }

}
