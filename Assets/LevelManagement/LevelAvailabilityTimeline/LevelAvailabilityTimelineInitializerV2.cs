using System;
using Timelines;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelAvailabilityTimelineInitializerV2", menuName = "Configuration/CoreGame/TimelineConfiguration/LevelAvailabilityTimelineInitializerV2", order = 1)]
    public class LevelAvailabilityTimelineInitializerV2 : TimelineInitializerV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>
    {
    }
}