using GameConfigurationID;
using UnityEngine;
using UnityEngine.Serialization;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ActionInteractableObjectInherentData", menuName = "Configuration/PuzzleGame/ActionInteractableObjectConfiguration/ActionInteractableObjectInherentData", order = 1)]
    public class ActionInteractableObjectInherentData : ScriptableObject
    {
        [WireCircle(R = 1, G = 0, B = 1)]
        public float InteractionRange;

        [CustomEnum(ConfigurationType = typeof(PlayerActionConfiguration))]
        public PlayerActionId PlayerActionId;

        public void Init(float InteractionRange)
        {
            this.InteractionRange = InteractionRange;
        }
    }
}
