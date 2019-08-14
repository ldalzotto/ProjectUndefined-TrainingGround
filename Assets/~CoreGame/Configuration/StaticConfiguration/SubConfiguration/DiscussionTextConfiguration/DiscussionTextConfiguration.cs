using ConfigurationEditor;
using GameConfigurationID;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTextConfiguration", menuName = "Configuration/CoreGame/DiscussionTextConfiguration/DiscussionTextConfiguration", order = 1)]
    public class DiscussionTextConfiguration : ConfigurationSerialization<DiscussionTextID, DiscussionTextInherentData>
    {

    }

}
