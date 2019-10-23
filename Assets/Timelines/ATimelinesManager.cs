using CoreGame;
using UnityEngine;

namespace Timelines
{
    public class ATimelinesManager : GameSingleton<ATimelinesManager>
    {
        private ITimelineNodeManager LevelAvailabilityTimeline;

        public void Init()
        {
            var aTimelinens = GameObject.FindObjectsOfType<ATimelineNodeManager>();
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