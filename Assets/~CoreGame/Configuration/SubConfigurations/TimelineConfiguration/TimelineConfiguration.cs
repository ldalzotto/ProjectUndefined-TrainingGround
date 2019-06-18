using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using System;
using GameConfigurationID;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TimelineConfiguration", menuName = "Configuration/CoreGame/TimelineConfiguration/TimelineConfiguration", order = 1)]

    public class TimelineConfiguration : ConfigurationSerialization<TimelineID, TimelineInitializerScriptableObject>
    {
    }
}
