using UnityEngine;
using System.Collections;
using System;

namespace CoreGame
{
    public class ATimelinesManager : MonoBehaviour
    {

        private ITimelineNodeManager ScenarioTimeline;
        private ITimelineNodeManager DiscussionTimeline;
        //V2
        private ITimelineNodeManager LevelAvailabilityTimeline;

        public void Init()
        {
            var aTimelinens = GameObject.FindObjectsOfType<ATimelineNodeManager>();
            foreach (var timeline in aTimelinens)
            {
                switch (timeline.GetTimelineID())
                {
                    case TimelineIDs.DISCUSSION_TIMELINE:
                        this.DiscussionTimeline = (ITimelineNodeManager)timeline;
                        break;
                    case TimelineIDs.LEVEL_AVAILABILITY_TIMELINE:
                        this.LevelAvailabilityTimeline = (ITimelineNodeManager)timeline;
                        break;
                    case TimelineIDs.SCENARIO_TIMELINE:
                        this.ScenarioTimeline = (ITimelineNodeManager)timeline;
                        break;
                }
            }
            this.InitTimelinesOnStart();
        }

        public ITimelineNodeManager[] GetAllTimelines()
        {
            return new ITimelineNodeManager[3]
            {
                this.ScenarioTimeline, this.DiscussionTimeline, this.LevelAvailabilityTimeline
            };
        }

        private void InitTimelinesOnStart()
        {
            this.LevelAvailabilityTimeline.Init();
        }

        public void InitTimelinesAtEndOfFrame()
        {
            this.ScenarioTimeline.Init();
            this.DiscussionTimeline.Init();
        }

        internal void ApplicationQuit()
        {
            this.PersistAllTimelines();
        }

        public void PersistAllTimelines()
        {
            foreach (var timeline in this.GetAllTimelines())
            {
                timeline.Persist();
            }
        }
    }

}
