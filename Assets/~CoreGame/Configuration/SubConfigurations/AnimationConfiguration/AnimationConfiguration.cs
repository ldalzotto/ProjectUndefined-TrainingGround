using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using System.Collections.Generic;
using GameConfigurationID;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AnimationConfiguration", menuName = "Configuration/CoreGame/AnimationConfiguration/AnimationConfiguration", order = 1)]
    public class AnimationConfiguration : ConfigurationSerialization<AnimationID, AnimationConfigurationData>
    {
    }

}
