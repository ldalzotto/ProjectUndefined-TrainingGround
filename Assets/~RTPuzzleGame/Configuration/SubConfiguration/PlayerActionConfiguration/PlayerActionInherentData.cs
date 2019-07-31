using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    public abstract class PlayerActionInherentData : ScriptableObject
    {
        [CustomEnum()]
        public SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId;
        public float CoolDownTime;
        [Tooltip("Number of times the action can be executed. -1 is infinite. -2 is not displayed")]
        public int ExecutionAmount = -1;

        protected PlayerActionInherentData(SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime)
        {
            ActionWheelNodeConfigurationId = actionWheelNodeConfigurationId;
            CoolDownTime = coolDownTime;
        }
    }


}
