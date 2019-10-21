using System;
using UnityEngine;

namespace CoreGame
{
    [Serializable]
    [CreateAssetMenu(fileName = "CoreConfiguration", menuName = "Configuration/CoreGame/CoreConfiguration", order = 1)]
    public class CoreConfiguration : GameConfiguration
    {
        public AnimationConfiguration AnimationConfiguration;
        public DiscussionTextConfiguration DiscussionTextConfiguration;
        public InputConfiguration InputConfiguration;
        public TimelineConfiguration TimelineConfiguration;
        public TutorialStepConfiguration TutorialStepConfiguration;
    }
}