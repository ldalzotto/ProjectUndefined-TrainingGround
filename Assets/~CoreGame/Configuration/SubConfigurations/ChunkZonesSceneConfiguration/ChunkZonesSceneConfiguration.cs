using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using System.Collections.Generic;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ChunkZonesSceneConfiguration", menuName = "Configuration/CoreGame/ChunkZonesSceneConfiguration/ChunkZonesSceneConfiguration", order = 1)]
    public class ChunkZonesSceneConfiguration : ConfigurationSerialization<LevelZoneChunkID, LevelZonesSceneConfigurationData>
    {
        public string GetSceneName(LevelZoneChunkID chunkID)
        {
            return ConfigurationInherentData[chunkID].sceneName;
        }

    }

}
