using UnityEngine;
using System.Collections;
using NodeGraph;
using System.Collections.Generic;
using System;
using UnityEditor;
using CoreGame;
using LevelManagement;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public class LevelCompletedTimelineActionNodeProfile : TimelineActionNodeProfile<LevelCompletedTimelineActionEdgeV2, LevelCompletedTimelineAction>
    {
    }
}