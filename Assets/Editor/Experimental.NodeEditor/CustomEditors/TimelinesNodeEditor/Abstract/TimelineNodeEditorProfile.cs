using UnityEngine;
using System.Collections;
using NodeGraph;
using CoreGame;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    public abstract class TimelineNodeEditorProfile : NodeEditorProfile
    {
        public abstract TimelineIDs TimelineID { get; }
    }
}