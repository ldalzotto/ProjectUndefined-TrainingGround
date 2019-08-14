using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class CoreConfigurationManager : MonoBehaviour
    {
        public CoreConfiguration CoreConfiguration;

        public LevelZonesSceneConfiguration LevelZonesSceneConfiguration() { return CoreConfiguration.LevelZonesSceneConfiguration; }
        public ChunkZonesSceneConfiguration ChunkZonesSceneConfiguration() { return CoreConfiguration.ChunkZonesSceneConfiguration; }
        public LevelHierarchyConfiguration LevelHierarchyConfiguration() { return CoreConfiguration.LevelHierarchyConfiguration; }
        public TimelineConfiguration TimelineConfiguration() { return CoreConfiguration.TimelineConfiguration; }
        public AnimationConfiguration AnimationConfiguration() { return CoreConfiguration.AnimationConfiguration; }
        public InputConfiguration InputConfiguration() { return CoreConfiguration.InputConfiguration; }

        public Dictionary<DiscussionTreeId, DiscussionTree> DiscussionConfiguration()
        {
            return CoreConfiguration.DiscussionTreeConfiguration.ConfigurationInherentData;
        }
        public Dictionary<InputID, InputConfigurationInherentData> InputConfigurationData()
        {
            return CoreConfiguration.InputConfiguration.ConfigurationInherentData;
        }
        public Dictionary<DiscussionTextID, DiscussionTextInherentData> DiscussionTextConfigurationData()
        {
            return CoreConfiguration.DiscussionTextConfiguration.ConfigurationInherentData;
        }
        public DiscussionTextConfiguration DiscussionTextConfiguration() { return CoreConfiguration.DiscussionTextConfiguration; }

        public TutorialStepConfiguration TutorialStepConfiguration() { return CoreConfiguration.TutorialStepConfiguration; }
        public Dictionary<TutorialStepID, TutorialStepInherentData> TutorialStepConfigurationData()
        {
            return CoreConfiguration.TutorialStepConfiguration.ConfigurationInherentData;
        }
    }

}
