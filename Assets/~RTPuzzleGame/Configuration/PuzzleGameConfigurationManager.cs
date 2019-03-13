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
            PuzzleGameConfiguration.AIComponentsConfiguration.Init();
        }

        public Dictionary<LaunchProjectileId, ProjectileInherentData> ProjectileConf()
        {
            return PuzzleGameConfiguration.ProjectileConfiguration.ConfigurationInherentData;
        }

        public Dictionary<TargetZoneID, TargetZoneInherentData> TargetZonesConfiguration()
        {
            return PuzzleGameConfiguration.TargetZonesConfiguration.ConfigurationInherentData;
        }

        public Dictionary<LevelZonesID, PlayerActionsInherentData> PlayerActionsConfiguration()
        {
            return PuzzleGameConfiguration.PlayerActionConfiguration.ConfigurationInherentData;
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
        public Dictionary<AiID, AIComponents> AIComponentsConfiguration()
        {
            return PuzzleGameConfiguration.AIComponentsConfiguration.ConfigurationInherentData;
        }
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleGameConfiguration", menuName = "Configuration/PuzzleGame/PuzzleGameConfiguration", order = 1)]
    public class PuzzleGameConfiguration : ScriptableObject
    {
        public ProjectileConfiguration ProjectileConfiguration;
        public TargetZonesConfiguration TargetZonesConfiguration;
        public PlayerActionConfiguration PlayerActionConfiguration;
        public AttractiveObjectConfiguration AttractiveObjectConfiguration;
        public LevelConfiguration LevelConfiguration;
        public SelectionWheelNodeConfiguration SelectionWheelNodeConfiguration;
        public AIComponentsConfiguration AIComponentsConfiguration;
    }

}
