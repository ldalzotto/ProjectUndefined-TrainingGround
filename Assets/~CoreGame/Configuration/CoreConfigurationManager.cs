using System.Collections.Generic;
using GameConfigurationID;
using UnityEngine;

namespace CoreGame
{
    public class CoreConfigurationManager : MonoBehaviour
    {
        public CoreConfiguration CoreConfiguration;

        public InputConfiguration InputConfiguration()
        {
            return CoreConfiguration.InputConfiguration;
        }

        public Dictionary<InputID, InputConfigurationInherentData> InputConfigurationData()
        {
            return CoreConfiguration.InputConfiguration.ConfigurationInherentData;
        }

        public Dictionary<DiscussionTextID, DiscussionTextInherentData> DiscussionTextConfigurationData()
        {
            return CoreConfiguration.DiscussionTextConfiguration.ConfigurationInherentData;
        }

        public DiscussionTextConfiguration DiscussionTextConfiguration()
        {
            return CoreConfiguration.DiscussionTextConfiguration;
        }
    }
}