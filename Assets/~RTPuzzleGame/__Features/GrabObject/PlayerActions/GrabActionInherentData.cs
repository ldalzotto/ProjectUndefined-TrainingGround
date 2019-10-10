using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "GrabActionInherentData", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/GrabActionInherentData", order = 1)]
    public class GrabActionInherentData : PlayerActionInherentData
    {
        [CustomEnum()]
        public PlayerActionId PlayerActionToIncrementOrAdd;

        public GrabActionInherentData(PlayerActionId PlayerActionToIncrementOrAdd, SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime) : base(actionWheelNodeConfigurationId, coolDownTime)
        {
            this.PlayerActionToIncrementOrAdd = PlayerActionToIncrementOrAdd;
        }

        public override RTPPlayerAction BuildPlayerAction()
        {
            return new GrabObjectAction(this);
        }
    }
}
