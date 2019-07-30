using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

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

        public Dictionary<AiID, ContextMarkVisualFeedbackInherentData> ContextMarkVisualFeedbackConfiguration()
        {
            return PuzzleGameConfiguration.ContextMarkVisualFeedbackConfiguration.ConfigurationInherentData;
        }

        public Dictionary<RangeTypeID, RangeTypeInherentConfigurationData> RangeTypeConfiguration()
        {
            return PuzzleGameConfiguration.RangeTypeConfiguration.ConfigurationInherentData;
        }

        public Dictionary<DottedLineID, DottedLineInherentData> DottedLineConfiguration()
        {
            return PuzzleGameConfiguration.DottedLineConfiguration.ConfigurationInherentData;
        }

        public Dictionary<RepelableObjectID, RepelableObjectsInherentData> RepelableObjectsConfiguration()
        {
            return PuzzleGameConfiguration.RepelableObjectsConfiguration.ConfigurationInherentData;
        }
        public Dictionary<DisarmObjectID, DisarmObjectInherentData> DisarmObjectsConfiguration()
        {
            return PuzzleGameConfiguration.DisarmObjectConfiguration.ConfigurationInherentData;
        }

        public Dictionary<PuzzleCutsceneId, PuzzleCutsceneInherentData> PuzzleCutsceneConfiguration()
        {
            return PuzzleGameConfiguration.PuzzleCutsceneConfiguration.ConfigurationInherentData;
        }
    }



}
