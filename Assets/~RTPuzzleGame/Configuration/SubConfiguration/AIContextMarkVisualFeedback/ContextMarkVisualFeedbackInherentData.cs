using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ContextMarkVisualFeedbackInherentData", menuName = "Configuration/PuzzleGame/ContextMarkVisualFeedbackConfiguration/ContextMarkVisualFeedbackInherentData", order = 1)]

    public class ContextMarkVisualFeedbackInherentData : ScriptableObject
    {
        public AIFeedbackMarkType AttractedPrafab;
        public AIFeedbackMarkType ProjectileHitPrefab;
        public AIFeedbackMarkType EscapeWithoutTargetPrefab;
    }

}
