using GameConfigurationID;
using UnityEngine;

namespace CoreGame
{
    public class ATimelinesManager : MonoBehaviour
    {
        private ITimelineNodeManager LevelAvailabilityTimeline;

        public void Init()
        {
            var aTimelinens = FindObjectsOfType<ATimelineNodeManager>();
            foreach (var timeline in aTimelinens)
                switch (timeline.GetTimelineID())
                {
                    case TimelineID.LEVEL_AVAILABILITY_TIMELINE:
                        LevelAvailabilityTimeline = (ITimelineNodeManager) timeline;
                        break;
                }

            InitTimelinesOnStart();
        }

        public ITimelineNodeManager[] GetAllTimelines()
        {
            return new ITimelineNodeManager[1]
            {
                LevelAvailabilityTimeline
            };
        }

        private void InitTimelinesOnStart()
        {
            LevelAvailabilityTimeline.Init();
        }

        public void InitTimelinesAtEndOfFrame()
        {
        }
    }
}