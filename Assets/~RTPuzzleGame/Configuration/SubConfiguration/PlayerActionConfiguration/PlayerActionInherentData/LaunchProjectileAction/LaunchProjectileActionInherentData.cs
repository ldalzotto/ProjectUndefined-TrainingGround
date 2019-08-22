using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LaunchProjectileActionInherentData", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/LaunchProjectileActionInherentData", order = 1)]
    public class LaunchProjectileActionInherentData : PlayerActionInherentData
    {
        [CustomEnum(ConfigurationType = typeof(InteractiveObjectTypeDefinitionConfiguration))]
        public InteractiveObjectTypeDefinitionID projectedObjectDefinitionID;

        public LaunchProjectileActionInherentData(InteractiveObjectTypeDefinitionID projectedObjectDefinitionID,
            SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime) : base(actionWheelNodeConfigurationId, coolDownTime)
        {
            this.projectedObjectDefinitionID = projectedObjectDefinitionID;
        }
        
    }
}
