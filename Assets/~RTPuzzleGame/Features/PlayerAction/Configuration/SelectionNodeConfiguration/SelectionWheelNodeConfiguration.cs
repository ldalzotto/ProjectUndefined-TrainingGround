using ConfigurationEditor;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "SelectionWheelNodeConfiguration", menuName = "Configuration/PuzzleGame/SelectionWheelNodeConfiguration/SelectionWheelNodeConfiguration", order = 1)]
    public class SelectionWheelNodeConfiguration : ConfigurationSerialization<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData>
    {
    }


}