using UnityEngine;
using System.Collections;
using NodeGraph;
using CoreGame;
using GameConfigurationID;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public abstract class TimelineNodeEditorProfile : NodeEditorProfile
    {
        public abstract TimelineID TimelineID { get; }
    }
}