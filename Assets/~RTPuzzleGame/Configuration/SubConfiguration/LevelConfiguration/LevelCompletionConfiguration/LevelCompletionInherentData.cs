using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelCompletionInherentData", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelCompletion/LevelCompletionInherentData", order = 1)]

    public class LevelCompletionInherentData : ScriptableObject
    {
        public LevelCompletionConditionConfiguration LevelCompletionConditionConfiguration;
    }


}
