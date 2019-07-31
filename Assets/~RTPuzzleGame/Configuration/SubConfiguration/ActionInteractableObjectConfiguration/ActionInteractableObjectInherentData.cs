using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ActionInteractableObjectInherentData", menuName = "Configuration/PuzzleGame/ActionInteractableObjectConfiguration/ActionInteractableObjectInherentData", order = 1)]
    public class ActionInteractableObjectInherentData : ScriptableObject
    {
        public float InteractionRange;
        [CustomEnum()]
        public PlayerActionId PlayerActionId;
        public InteractiveObjectType ActionInteractableObjectPrefab;

        public void Init(float InteractionRange)
        {
            this.InteractionRange = InteractionRange;
        }
    }
}
