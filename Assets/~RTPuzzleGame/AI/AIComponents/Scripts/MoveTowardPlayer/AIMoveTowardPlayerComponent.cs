using UnityEngine;
using static AIMovementDefinitions;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "AIMoveTowardPlayerComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIMoveTowardPlayerComponent", order = 1)]
    public class AIMoveTowardPlayerComponent : AbstractAIComponent
    {
        public AIMovementSpeedDefinition AISpeed;
    }

}
