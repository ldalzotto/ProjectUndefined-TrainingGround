using ConfigurationEditor;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelConfiguration", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelConfiguration", order = 1)]
    public class LevelConfiguration : ConfigurationSerialization<LevelZonesID, LevelConfigurationData>
    {
        /*
        public static Dictionary<LevelZonesID, LevelConfigurationData> conf = new Dictionary<LevelZonesID, LevelConfigurationData>()
        {
            { LevelZonesID.SEWER_RTP, new LevelConfigurationData(20f) }
        };
        */
    }
}
