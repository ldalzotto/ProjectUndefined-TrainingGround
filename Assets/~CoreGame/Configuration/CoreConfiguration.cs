using System;
using UnityEngine;

namespace CoreGame
{
    [Serializable]
    [CreateAssetMenu(fileName = "CoreConfiguration", menuName = "Configuration/CoreGame/CoreConfiguration", order = 1)]
    public class CoreConfiguration : GameConfiguration
    {
        public DiscussionTextConfiguration DiscussionTextConfiguration;
        public InputConfiguration InputConfiguration;
    }
}