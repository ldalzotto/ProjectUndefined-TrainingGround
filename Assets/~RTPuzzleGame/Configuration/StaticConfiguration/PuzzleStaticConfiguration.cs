using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleStaticConfiguration", menuName = "Configuration/PuzzleGame/StaticConfiguration/PuzzleStaticConfiguration", order = 1)]
    public class PuzzleStaticConfiguration : ScriptableObject
    {
        public RangeColorConfiguration RangeColorConfiguration;
        public PuzzlePrefabConfiguration PuzzlePrefabConfiguration;
        public PuzzleMaterialConfiguration PuzzleMaterialConfiguration;
        public PuzzleGlobalStaticConfiguration PuzzleGlobalStaticConfiguration;
    }
}

