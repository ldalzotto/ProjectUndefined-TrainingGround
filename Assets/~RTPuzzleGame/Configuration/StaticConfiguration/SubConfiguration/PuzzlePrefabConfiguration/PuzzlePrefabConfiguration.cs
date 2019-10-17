using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzlePrefabConfiguration", menuName = "Configuration/PuzzleGame/StaticConfiguration/PuzzlePrefabConfiguration", order = 1)]
    public class PuzzlePrefabConfiguration : ScriptableObject
    {
        public DottedLine BaseDottedLineBasePrefab;
    }
}

