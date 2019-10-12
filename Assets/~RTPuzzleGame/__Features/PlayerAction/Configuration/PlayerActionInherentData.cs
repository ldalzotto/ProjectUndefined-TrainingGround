using CoreGame;
using GameConfigurationID;
using InteractiveObjectTest;
using OdinSerializer;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    public abstract class PlayerActionInherentData : SerializedScriptableObject
    {
        public CorePlayerActionDefinition CorePlayerActionDefinition;

        public abstract RTPPlayerAction BuildPlayerAction(PlayerInteractiveObject PlayerInteractiveObject);
    }

    [System.Serializable]
    public struct CorePlayerActionDefinition
    {
        [CustomEnum()]
        public PlayerActionType PlayerActionType;

        [CustomEnum(ConfigurationType = typeof(SelectionWheelNodeConfiguration))]
        public SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId;

        [Tooltip("Number of times the action can be executed. -1 is infinite. -2 is not displayed")]
        public int ExecutionAmount;
        public float CoolDownTime;
    }

}
