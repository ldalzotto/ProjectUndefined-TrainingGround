using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameConfigurationID;

namespace CoreGame
{
    public class CoreConfigurationManager : MonoBehaviour
    {
        public CoreConfiguration CoreConfiguration;

        public LevelZonesSceneConfiguration LevelZonesSceneConfiguration() { return CoreConfiguration.LevelZonesSceneConfiguration; }
        public ChunkZonesSceneConfiguration ChunkZonesSceneConfiguration() { return CoreConfiguration.ChunkZonesSceneConfiguration; }
        public LevelHierarchyConfiguration LevelHierarchyConfiguration() { return CoreConfiguration.LevelHierarchyConfiguration; }
        public TimelineConfiguration TimelineConfiguration() { return CoreConfiguration.TimelineConfiguration; }
        public AnimationConfiguration AnimationConfiguration() { return CoreConfiguration.AnimationConfiguration; }
        public Dictionary<DiscussionTreeId, DiscussionTree> DiscussionConfiguration()
        {
            return CoreConfiguration.DiscussionTreeConfiguration.ConfigurationInherentData;
        }
    }

}
