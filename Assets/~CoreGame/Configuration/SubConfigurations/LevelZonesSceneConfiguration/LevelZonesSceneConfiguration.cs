using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using System.Collections.Generic;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelZonesSceneConfiguration", menuName = "Configuration/CoreGame/LevelZonesSceneConfiguration/LevelZonesSceneConfiguration", order = 1)]
    public class LevelZonesSceneConfiguration : ConfigurationSerialization<LevelZonesID, LevelZonesSceneConfigurationData>
    {
        public string GetSceneName(LevelZonesID levelZonesID)
        {
            return ConfigurationInherentData[levelZonesID].sceneName;
        }

        public List<LevelZoneChunkID> GetLevelHierarchy(LevelZonesID levelZonesID)
        {
            return ConfigurationInherentData[levelZonesID].LevelHierarchy;
        }
    }
}

