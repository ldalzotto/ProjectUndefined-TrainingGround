using UnityEngine;
using System.Collections;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CoreConfiguration", menuName = "Configuration/CoreGame/CoreConfiguration", order = 1)]
    public class CoreConfiguration : GameConfiguration
    {
        public LevelZonesSceneConfiguration LevelZonesSceneConfiguration;
        public ChunkZonesSceneConfiguration ChunkZonesSceneConfiguration;
        public LevelHierarchyConfiguration LevelHierarchyConfiguration;
        public TimelineConfiguration TimelineConfiguration;
        public AnimationConfiguration AnimationConfiguration;
        public DiscussionTreeConfiguration DiscussionTreeConfiguration;
        public InputConfiguration InputConfiguration;
        public DiscussionTextConfiguration DiscussionTextConfiguration;
        public TutorialStepConfiguration TutorialStepConfiguration;
        public SelectionWheelNodeConfiguration SelectionWheelNodeConfiguration;
    }
}

