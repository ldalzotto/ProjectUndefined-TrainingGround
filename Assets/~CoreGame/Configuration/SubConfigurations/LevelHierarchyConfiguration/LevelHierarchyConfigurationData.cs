using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelHierarchyConfigurationData", menuName = "Configuration/CoreGame/LevelHierarchyConfiguration/LevelHierarchyConfigurationData", order = 1)]
    public class LevelHierarchyConfigurationData : ScriptableObject
    {
        [ReorderableListAttribute]
        public List<LevelZoneChunkID> LevelHierarchy;
    }

}
