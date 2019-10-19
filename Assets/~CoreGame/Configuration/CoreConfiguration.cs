using System;
using UnityEngine;

namespace CoreGame
{
    [Serializable]
    [CreateAssetMenu(fileName = "CoreConfiguration", menuName = "Configuration/CoreGame/CoreConfiguration", order = 1)]
    public class CoreConfiguration : GameConfiguration
    {
        public AnimationConfiguration AnimationConfiguration;
        public ChunkZonesSceneConfiguration ChunkZonesSceneConfiguration;
        public DiscussionTextConfiguration DiscussionTextConfiguration;
        public InputConfiguration InputConfiguration;
        public LevelHierarchyConfiguration LevelHierarchyConfiguration;
        public LevelZonesSceneConfiguration LevelZonesSceneConfiguration;
        public SelectionWheelNodeConfiguration SelectionWheelNodeConfiguration;
        public TimelineConfiguration TimelineConfiguration;
        public TutorialStepConfiguration TutorialStepConfiguration;
    }
}