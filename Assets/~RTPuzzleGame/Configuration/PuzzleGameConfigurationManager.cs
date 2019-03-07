using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTPuzzle
{

    public class PuzzleGameConfigurationManager : MonoBehaviour
    {
        public PuzzleGameConfiguration PuzzleGameConfiguration;

        public void Init()
        {
            PuzzleGameConfiguration.PlayerActionConfiguration.Init();
        }

        public Dictionary<LaunchProjectileId, ProjectileInherentData> ProjectileConf()
        {
            return PuzzleGameConfiguration.ProjectileConfiguration.LaunchProjectileInherentDatas;
        }

        public Dictionary<TargetZoneID, TargetZoneInherentData> TargetZonesConfiguration()
        {
            return PuzzleGameConfiguration.TargetZonesConfiguration.conf;
        }

        public Dictionary<LevelZonesID, PlayerActionsInherentData> PlayerActionsConfiguration()
        {
            return PuzzleGameConfiguration.PlayerActionConfiguration.conf;
        }
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleGameConfiguration", menuName = "Configuration/PuzzleGame/PuzzleGameConfiguration", order = 1)]
    public class PuzzleGameConfiguration : ScriptableObject
    {
        public ProjectileConfiguration ProjectileConfiguration;
        public TargetZonesConfiguration TargetZonesConfiguration;
        public PlayerActionConfiguration PlayerActionConfiguration;
    }

}
