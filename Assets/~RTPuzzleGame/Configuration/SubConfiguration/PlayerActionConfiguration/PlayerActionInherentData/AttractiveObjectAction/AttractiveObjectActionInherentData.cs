using UnityEngine;
using UnityEditor;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AttractiveObjectActionInherentData", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/AttractiveObjectActionInherentData", order = 1)]
    public class AttractiveObjectActionInherentData : PlayerActionInherentData
    {
        public AttractiveObjectId AttractiveObjectId;

        public AttractiveObjectActionInherentData(AttractiveObjectId AttractiveObjectId, 
            SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime) : base(actionWheelNodeConfigurationId, coolDownTime)
        {
            this.AttractiveObjectId = AttractiveObjectId;
        }
    }
}
