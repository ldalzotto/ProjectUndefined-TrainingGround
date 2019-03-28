using UnityEngine;
using System.Collections;
using OdinSerializer;
using System;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AiBehaviorInherentData", menuName = "Configuration/PuzzleGame/AiBehaviorInherentData", order = 1)]
    public class AIBehaviorInherentData : SerializedScriptableObject
    {
        public Type BehaviorType;
        public AbstractAIComponents AIComponents;
    }
}
