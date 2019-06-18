using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LaunchProjectileActionInherentData", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/LaunchProjectileActionInherentData", order = 1)]
    public class LaunchProjectileActionInherentData : PlayerActionInherentData
    {
        public LaunchProjectileId launchProjectileId;

        public LaunchProjectileActionInherentData(LaunchProjectileId launchProjectileId,
            SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime) : base(actionWheelNodeConfigurationId, coolDownTime)
        {
            this.launchProjectileId = launchProjectileId;
        }
    }
}
