using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class PuzzleStaticConfigurationContainer : MonoBehaviour
    {
        public PuzzleStaticConfiguration PuzzleStaticConfiguration;

        public PuzzlePrefabConfiguration GetPuzzlePrefabConfiguration() { return this.PuzzleStaticConfiguration.PuzzlePrefabConfiguration; }
    }
}
