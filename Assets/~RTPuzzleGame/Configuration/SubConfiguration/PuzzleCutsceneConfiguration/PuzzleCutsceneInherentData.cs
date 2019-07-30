using GameConfigurationID;
using OdinSerializer;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleCutsceneInherentData", menuName = "Configuration/PuzzleGame/PuzzleCutsceneConfiguration/PuzzleCutsceneInherentData", order = 1)]
    public class PuzzleCutsceneInherentData : SerializedScriptableObject
    {
        public PuzzleCutsceneGraph PuzzleCutsceneGraph;
    }
}