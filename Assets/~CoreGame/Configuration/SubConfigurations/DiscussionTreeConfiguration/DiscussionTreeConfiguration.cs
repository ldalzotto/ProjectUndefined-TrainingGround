using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTreeConfiguration", menuName = "Configuration/CoreGame/DiscussionTreeConfiguration/DiscussionTreeConfiguration", order = 1)]
    public class DiscussionTreeConfiguration : ConfigurationSerialization<DiscussionTreeId, DiscussionTree>
    {
        
    }

}
