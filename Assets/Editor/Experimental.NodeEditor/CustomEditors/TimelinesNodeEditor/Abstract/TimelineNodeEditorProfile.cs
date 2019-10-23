using System;
using NodeGraph;
using Timelines;

namespace Editor_LevelAvailabilityNodeEditor
{
    [Serializable]
    public abstract class TimelineNodeEditorProfile : NodeEditorProfile
    {
        public abstract TimelineID TimelineID { get; }
    }
}