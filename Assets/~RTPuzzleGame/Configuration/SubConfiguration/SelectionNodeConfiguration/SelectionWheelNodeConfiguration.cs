using ConfigurationEditor;
using UnityEngine;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "SelectionWheelNodeConfiguration", menuName = "Configuration/PuzzleGame/SelectionWheelNodeConfiguration/SelectionWheelNodeConfiguration", order = 1)]
    public class SelectionWheelNodeConfiguration : ConfigurationSerialization<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData>
    {
        /*
        public static Dictionary<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData> selectionWheelNodeConfiguration =
            new Dictionary<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData>()
            {
                    {SelectionWheelNodeConfigurationId.THROW_PLAYER_PUZZLE_WHEEL_CONFIG, new SelectionWheelNodeConfigurationData(typeof(LaunchProjectileAction)) },
                    {SelectionWheelNodeConfigurationId.ATTRACTIVE_OBJECT_LAY_WHEEL_CONFIG, new SelectionWheelNodeConfigurationData(typeof(AttractiveObjectAction)) }
            };
            */
    }

    public enum SelectionWheelNodeConfigurationId
    {
        THROW_PLAYER_PUZZLE_WHEEL_CONFIG = 3,
        ATTRACTIVE_OBJECT_LAY_WHEEL_CONFIG = 4
    }

}