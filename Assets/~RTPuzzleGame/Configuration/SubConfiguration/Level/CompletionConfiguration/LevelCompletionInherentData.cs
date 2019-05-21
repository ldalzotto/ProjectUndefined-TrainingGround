using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelCompletionInherentData", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelCompletion/LevelCompletionInherentData", order = 1)]

    public class LevelCompletionInherentData : ScriptableObject
    {
        public ConditionGraphEditorProfile ConditionGraphEditorProfile;
    }

}
