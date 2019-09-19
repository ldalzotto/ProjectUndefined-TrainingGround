using UnityEngine;
using UnityEditor;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AttractiveObjectActionInherentData", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/AttractiveObjectActionInherentData", order = 1)]
    public class AttractiveObjectActionInherentData : PlayerActionInherentData
    {
        [CustomEnum(ConfigurationType = typeof(InteractiveObjectTypeDefinitionConfiguration))]
        public InteractiveObjectTypeDefinitionID AttractiveObjectDefinitionID;

        public AttractiveObjectActionInherentData(SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime) : base(actionWheelNodeConfigurationId, coolDownTime)
        {
        }

        public override RTPPlayerAction BuildPlayerAction()
        {
            return new AttractiveObjectAction(this);
        }
    }
}
