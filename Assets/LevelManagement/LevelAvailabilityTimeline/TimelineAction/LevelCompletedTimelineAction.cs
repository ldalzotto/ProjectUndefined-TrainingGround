﻿using System;
using Timelines;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    public struct LevelCompletedTimelineAction : TimeLineAction
    {
        [SerializeField] private LevelZonesID completedLevelZone;

        public LevelCompletedTimelineAction(LevelZonesID completedLevelZone)
        {
            this.completedLevelZone = completedLevelZone;
        }

        public override bool Equals(object obj)
        {
            return obj is LevelCompletedTimelineAction action &&
                   completedLevelZone == action.completedLevelZone;
        }

        public override int GetHashCode()
        {
            return -1721677994 + completedLevelZone.GetHashCode();
        }
#if UNITY_EDITOR
        public void NodeGUI()
        {
        }
#endif
    }
}