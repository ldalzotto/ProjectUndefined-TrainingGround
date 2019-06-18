using UnityEngine;
using System.Collections;
using OdinSerializer;
using System;
using System.Collections.Generic;
using GameConfigurationID;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelAvailabilityTimelineInitializerV2", menuName = "Configuration/CoreGame/TimelineConfiguration/LevelAvailabilityTimelineInitializerV2", order = 1)]
    public class LevelAvailabilityTimelineInitializerV2 : TimelineInitializerV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>
    {
    }

}
