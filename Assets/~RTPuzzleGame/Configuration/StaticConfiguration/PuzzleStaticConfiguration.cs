using System;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "PuzzleStaticConfiguration", menuName = "Configuration/PuzzleGame/StaticConfiguration/PuzzleStaticConfiguration", order = 1)]
    public class PuzzleStaticConfiguration : ScriptableObject
    {
        public RangeColorConfiguration RangeColorConfiguration;
        public PuzzleMaterialConfiguration PuzzleMaterialConfiguration;
    }
}