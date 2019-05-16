using UnityEngine;
using System.Collections;
using System;

namespace CoreGame
{
    public class ATimelinesManager : MonoBehaviour
    {
        private ITimelineNodeManager[] AllTimelines;

        public void Init()
        {
            var aTimelinens = GameObject.FindObjectsOfType<ATimelineNodeManager>();
            this.AllTimelines = new ITimelineNodeManager[aTimelinens.Length];
            for (var i = 0; i < aTimelinens.Length; i++)
            {
                this.AllTimelines[i] = (ITimelineNodeManager)aTimelinens[i];
            }
        }

        public void InitAllTimelines()
        {
            foreach (var timeline in this.AllTimelines)
            {
                timeline.Init();
            }
        }

        internal void ApplicationQuit()
        {
            this.PersistAllTimelines();
        }

        public void PersistAllTimelines()
        {
            foreach (var timeline in this.AllTimelines)
            {
                timeline.Persist();
            }
        }
    }

}
