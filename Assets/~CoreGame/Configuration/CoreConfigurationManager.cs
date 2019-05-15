using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public class CoreConfigurationManager : MonoBehaviour
    {
        public CoreConfiguration CoreConfiguration;

        public LevelZonesSceneConfiguration LevelZonesSceneConfiguration() { return CoreConfiguration.LevelZonesSceneConfiguration; }
        public ChunkZonesSceneConfiguration ChunkZonesSceneConfiguration() { return CoreConfiguration.ChunkZonesSceneConfiguration; }
        public LevelHierarchyConfiguration LevelHierarchyConfiguration() { return CoreConfiguration.LevelHierarchyConfiguration; }
    }

}
