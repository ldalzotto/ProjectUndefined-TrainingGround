using UnityEngine;
using UnityEditor;

namespace RTPuzzle
{
    [System.Serializable]
    public abstract class PlayerActionInherentData : ScriptableObject
    {
        public SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId;
        public float CoolDownTime;

        protected PlayerActionInherentData(SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime)
        {
            ActionWheelNodeConfigurationId = actionWheelNodeConfigurationId;
            CoolDownTime = coolDownTime;
        }
    }

    public enum PlayerActionId
    {
        STONE_PROJECTILE_ACTION_1 = 0,
        CHEESE_ATTRACTIVE_ACTION_1 = 1,
        ATTRACTIVE_ACTION_EDITOR_TEST = 2
    }
}
