using System;
using System.Collections.Generic;
using CoreGame;

namespace Timelines
{
    public class ATimelinePersister<T> : AbstractGamePersister<T>
    {
        public const string TimelinePersisterFolderName = "Timelines";
        public const string TimelineFileExtension = ".tim";

        public ATimelinePersister(Type timelineType) : base(TimelinePersisterFolderName, TimelineFileExtension, timelineType.Name)
        {
        }
    }
}

namespace Persistence
{
    [Serializable]
    public struct ATimelinePersistedNodes<NODE_KEY>
    {
        public List<NODE_KEY> Nodes;
    }
}