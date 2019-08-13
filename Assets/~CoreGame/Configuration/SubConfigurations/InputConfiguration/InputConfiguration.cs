using UnityEngine;
using ConfigurationEditor;
using System.Collections.Generic;
using GameConfigurationID;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "InputConfiguration", menuName = "Configuration/CoreGame/InputConfiguration/InputConfiguration", order = 1)]
    public class InputConfiguration : ConfigurationSerialization<InputID, InputConfigurationInherentData>
    {
    }

}

