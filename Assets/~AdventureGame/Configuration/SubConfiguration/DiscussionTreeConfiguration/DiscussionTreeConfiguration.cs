using UnityEngine;
using System.Collections;
using ConfigurationEditor;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTreeConfiguration", menuName = "Configuration/AdventureGame/DiscussionTreeConfiguration/DiscussionTreeConfiguration", order = 1)]
    public class DiscussionTreeConfiguration : ConfigurationSerialization<DiscussionTreeId, DiscussionTree>
    {
        
    }

    public enum DiscussionTreeId
    {
        BOUNCER_DISCUSSION_TREE,
        BOUNCER_OK_DISCUSSION
    }

}
