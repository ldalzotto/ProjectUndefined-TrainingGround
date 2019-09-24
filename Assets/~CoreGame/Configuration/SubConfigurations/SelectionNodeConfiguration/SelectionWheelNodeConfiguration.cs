using ConfigurationEditor;
using GameConfigurationID;
using UnityEngine;

namespace CoreGame
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "SelectionWheelNodeConfiguration", menuName = "Configuration/CoreGame/SelectionWheelNodeConfiguration/SelectionWheelNodeConfiguration", order = 1)]
    public class SelectionWheelNodeConfiguration : ConfigurationSerialization<SelectionWheelNodeConfigurationId, SelectionWheelNodeConfigurationData>
    {
    }


}