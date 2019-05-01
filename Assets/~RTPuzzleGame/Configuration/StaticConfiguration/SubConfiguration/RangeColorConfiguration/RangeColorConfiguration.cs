using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RangeColorConfiguration", menuName = "Configuration/PuzzleGame/StaticConfiguration/RangeColorConfiguration", order = 1)]
    public class RangeColorConfiguration : ScriptableObject
    {
        [ColorUsage(true, true)]
        public Color ProjectileCursorOnRangeColor;
        [ColorUsage(true, true)]
        public Color ProjectileCursorOutOfRangeColor;
    }
}