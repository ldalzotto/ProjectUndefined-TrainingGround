using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LaunchProjectileActionInherentData", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/LaunchProjectileActionInherentData", order = 1)]
    public class LaunchProjectileActionInherentData : PlayerActionInherentData
    {
        public LaunchProjectileID launchProjectileId;

        public LaunchProjectileActionInherentData(LaunchProjectileID launchProjectileId,
            SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime) : base(actionWheelNodeConfigurationId, coolDownTime)
        {
            this.launchProjectileId = launchProjectileId;
        }
    }
}
