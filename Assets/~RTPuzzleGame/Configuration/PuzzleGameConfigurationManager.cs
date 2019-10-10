using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{

    public class PuzzleGameConfigurationManager : MonoBehaviour
    {
        public PuzzleGameConfiguration PuzzleGameConfiguration;
                
        public Dictionary<PlayerActionId, PlayerActionInherentData> PlayerActionConfiguration()
        {
            return PuzzleGameConfiguration.PlayerActionConfiguration.ConfigurationInherentData;
        }
        
        public Dictionary<LevelZonesID, LevelConfigurationData> LevelConfiguration()
        {
            return PuzzleGameConfiguration.LevelConfiguration.ConfigurationInherentData;
        }

        public Dictionary<RangeTypeID, RangeTypeInherentConfigurationData> RangeTypeConfiguration()
        {
            return PuzzleGameConfiguration.RangeTypeConfiguration.ConfigurationInherentData;
        }

        public Dictionary<DottedLineID, DottedLineInherentData> DottedLineConfiguration()
        {
            return PuzzleGameConfiguration.DottedLineConfiguration.ConfigurationInherentData;
        }

        public Dictionary<PuzzleCutsceneID, PuzzleCutsceneInherentData> PuzzleCutsceneConfiguration()
        {
            return PuzzleGameConfiguration.PuzzleCutsceneConfiguration.ConfigurationInherentData;
        }

//${addNewEntry}
    }



}
