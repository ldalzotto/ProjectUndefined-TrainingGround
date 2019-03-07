using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTPuzzle
{

    public class PuzzleGameConfigurationManager : MonoBehaviour
    {
        public PuzzleGameConfiguration PuzzleGameConfiguration;

        public Dictionary<LaunchProjectileId, ProjectileInherentData> ProjectileConf()
        {
            return PuzzleGameConfiguration.ProjectileConfiguration.LaunchProjectileInherentDatas;
        }

        public Dictionary<TargetZoneID, TargetZoneInherentData> TargetZonesConfiguration()
        {
            return PuzzleGameConfiguration.TargetZonesConfiguration.conf;
        }
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleGameConfiguration", menuName = "Configuration/PuzzleGame/PuzzleGameConfiguration", order = 1)]
    public class PuzzleGameConfiguration : ScriptableObject
    {
        public ProjectileConfiguration ProjectileConfiguration;
        public TargetZonesConfiguration TargetZonesConfiguration;
    }

}
