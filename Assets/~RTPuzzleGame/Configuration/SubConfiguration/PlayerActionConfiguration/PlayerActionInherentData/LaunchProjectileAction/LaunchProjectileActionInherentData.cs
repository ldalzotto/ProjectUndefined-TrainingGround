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

        public LaunchProjectileID launchProjectileId;

        public LaunchProjectileActionInherentData(LaunchProjectileID launchProjectileId, InteractiveObjectTypeDefinitionID projectedObjectDefinitionID,
            SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime) : base(actionWheelNodeConfigurationId, coolDownTime)
        {
            this.launchProjectileId = launchProjectileId;
            this.projectedObjectDefinitionID = projectedObjectDefinitionID;
        }
        
    }
}
