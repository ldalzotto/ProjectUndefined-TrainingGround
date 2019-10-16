using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleMaterialConfiguration", menuName = "Configuration/PuzzleGame/StaticConfiguration/PuzzleMaterialConfiguration", order = 1)]
    public class PuzzleMaterialConfiguration : ScriptableObject
    {
        [Header("Range Material")]
        public Material MasterRangeMaterial;
        public Mesh RangeDiamondMesh;

        [Header("Visual Feedback Material")]
        public Shader BaseDottedLineShader;
    }
}

