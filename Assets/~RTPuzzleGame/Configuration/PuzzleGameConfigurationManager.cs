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
            PuzzleGameConfiguration.LevelConfiguration.Init(PuzzleGameConfiguration.PlayerActionConfiguration);
        }

        public Dictionary<LaunchProjectileId, ProjectileInherentData> ProjectileConf()
        {
            return PuzzleGameConfiguration.ProjectileConfiguration.ConfigurationInherentData;
        }

        public Dictionary<TargetZoneID, TargetZoneInherentData> TargetZonesConfiguration()
        {
            return PuzzleGameConfiguration.TargetZonesConfiguration.ConfigurationInherentData;
        }

        public Dictionary<AttractiveObjectId, AttractiveObjectInherentConfigurationData> AttractiveObjectsConfiguration()
        {
            return PuzzleGameConfiguration.AttractiveObjectConfiguration.ConfigurationInherentData;
        }
        public Dictionary<LevelZonesID, LevelConfigurationData> LevelConfiguration()
        {
            return PuzzleGameConfiguration.LevelConfiguration.ConfigurationInherentData;
        }
        public Dictionary<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData> SelectionWheelNodeConfiguration()
        {
            return PuzzleGameConfiguration.SelectionWheelNodeConfiguration.ConfigurationInherentData;
        }
        public Dictionary<AiID, AIBehaviorInherentData> AIComponentsConfiguration()
        {
            return PuzzleGameConfiguration.AIComponentsConfiguration.ConfigurationInherentData;
        }
    }



}
