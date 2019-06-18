using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTreeConfiguration", menuName = "Configuration/AdventureGame/DiscussionTreeConfiguration/DiscussionTreeConfiguration", order = 1)]
    public class DiscussionTreeConfiguration : ConfigurationSerialization<DiscussionTreeId, DiscussionTree>
    {
        
    }

}
