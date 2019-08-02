using GameConfigurationID;
using UnityEngine;
using UnityEngine.Serialization;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ActionInteractableObjectInherentData", menuName = "Configuration/PuzzleGame/ActionInteractableObjectConfiguration/ActionInteractableObjectInherentData", order = 1)]
    public class ActionInteractableObjectInherentData : ScriptableObject
    {
        public float InteractionRange;
        [CustomEnum()]
        public PlayerActionId PlayerActionId;

        [SerializeField]
        [FormerlySerializedAs("ActionInteractableObjectPrefab")]
        public InteractiveObjectType AssociatedInteractiveObjectType;

        public void Init(float InteractionRange)
        {
            this.InteractionRange = InteractionRange;
        }
    }
}
