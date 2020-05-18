using System.Collections.Generic;
using GameConfigurationID;
using RangeObjects;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleGameConfigurationManager : MonoBehaviour
    {
        public PuzzleGameConfiguration PuzzleGameConfiguration;

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
    }
}