using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using System.Collections.Generic;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelHierarchyConfiguration", menuName = "Configuration/CoreGame/LevelHierarchyConfiguration/LevelHierarchyConfiguration", order = 1)]
    public class LevelHierarchyConfiguration : ConfigurationSerialization<LevelZonesID, LevelHierarchyConfigurationData>
    {
        public List<LevelZoneChunkID> GetLevelHierarchy(LevelZonesID levelZonesID)
        {
            if (ConfigurationInherentData[levelZonesID] == null)
            {
                return new List<LevelZoneChunkID>();
            }
            else
            {
                return ConfigurationInherentData[levelZonesID].LevelHierarchy;
            }
        }
    }

}

