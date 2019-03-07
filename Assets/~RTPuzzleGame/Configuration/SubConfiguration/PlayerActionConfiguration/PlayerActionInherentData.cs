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
}
