using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System;
using System.IO;

namespace CoreGame
{
    public class ATimelinePersister<T> : AbstractGamePersister<List<T>>
    {
        public const string TimelinePersisterFolderName = "Timelines";
        public const string TimelineFileExtension = ".tim";

        public ATimelinePersister(Type timelineType) : base(TimelinePersisterFolderName, TimelineFileExtension, timelineType.Name)
        {
        }
        
    }

}
