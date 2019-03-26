using ConfigurationEditor;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelConfiguration", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelConfiguration", order = 1)]
    public class LevelConfiguration : ConfigurationSerialization<LevelZonesID, LevelConfigurationData>
    {
        public void Init(PlayerActionConfiguration playerActionConfiguration)
        {
            foreach (var lovelConfigurationData in ConfigurationInherentData)
            {
                lovelConfigurationData.Value.Init(playerActionConfiguration);
            }
        }
    }
}
