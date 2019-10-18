using System;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "PuzzleMaterialConfiguration", menuName = "Configuration/PuzzleGame/StaticConfiguration/PuzzleMaterialConfiguration", order = 1)]
    public class PuzzleMaterialConfiguration : ScriptableObject
    {
        [Header("Visual Feedback Material")] public Shader BaseDottedLineShader;
        [Header("Line Visual Feedback")] public Mesh RangeDiamondMesh;
    }
}