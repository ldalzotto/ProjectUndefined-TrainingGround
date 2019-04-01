using ConfigurationEditor;
using UnityEngine;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "SelectionWheelNodeConfiguration", menuName = "Configuration/PuzzleGame/SelectionWheelNodeConfiguration/SelectionWheelNodeConfiguration", order = 1)]
    public class SelectionWheelNodeConfiguration : ConfigurationSerialization<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData>
    {
    }

    public enum SelectionWheelNodeConfigurationId
    {
        THROW_PLAYER_PUZZLE_WHEEL_CONFIG = 3,
        ATTRACTIVE_OBJECT_LAY_WHEEL_CONFIG = 4,
        EDITOR_TEST = 5
    }

}