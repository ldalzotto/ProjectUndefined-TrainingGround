using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "NearPlayerGameOverTriggerInherentData", menuName = "Configuration/PuzzleGame/NearPlayerGameOverTriggerConfiguration/NearPlayerGameOverTriggerInherentData", order = 1)]
    public class NearPlayerGameOverTriggerInherentData : ScriptableObject
    {
        [WireCircle(R = 1, G = 0, B = 1)]
        public float NearPlayerDetectionRadius;

        [CustomEnum(ConfigurationType = typeof(PuzzleCutsceneConfiguration))]
        public PuzzleCutsceneID AnimationCutsceneGraphID;
    }

}
