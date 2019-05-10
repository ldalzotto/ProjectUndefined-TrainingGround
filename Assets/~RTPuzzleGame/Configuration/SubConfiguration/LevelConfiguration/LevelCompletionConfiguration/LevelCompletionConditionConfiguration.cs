using UnityEngine;
using System.Collections;
using CoreGame;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelCompletionConditionConfiguration", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelCompletion/LevelCompletionConditionConfiguration", order = 1)]
    public class LevelCompletionConditionConfiguration : ConditionGraph
    {
        public override ConditionNode NodeProvider()
        {
            return new LevelCompletionConditionNode();
        }
    }

}
