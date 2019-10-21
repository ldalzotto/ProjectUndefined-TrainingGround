using System.Collections.Generic;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleGameConfigurationManager : MonoBehaviour
    {
        public PuzzleGameConfiguration PuzzleGameConfiguration;

        public Dictionary<DottedLineID, DottedLineInherentData> DottedLineConfiguration()
        {
            return PuzzleGameConfiguration.DottedLineConfiguration.ConfigurationInherentData;
        }
    }
}