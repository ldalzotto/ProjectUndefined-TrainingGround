﻿using UnityEngine;
using UnityEditor;
using OdinSerializer;
using System.Collections.Generic;
using NodeGraph;
using System;
using CoreGame;
using GameConfigurationID;
using LevelManagement;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public class LevelAvailabilityNodeProfile : TimelineNodeProfile<LevelAvailabilityTimelineNodeID>
    {
    }
}