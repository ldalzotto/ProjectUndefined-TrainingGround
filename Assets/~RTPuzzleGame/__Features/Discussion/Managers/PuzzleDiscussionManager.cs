using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleDiscussionManager : MonoBehaviour, IPuzzleDiscussionManagerEvent
    {
        private PuzzleDiscussionWindowsContainer PuzzleDiscussionWindowsContainer;

        public void Init()
        {
            this.PuzzleDiscussionWindowsContainer = PuzzleGameSingletonInstances.PuzzleDiscussionWindowsContainer;
            this.PuzzleDiscussionWindowsContainer.Init();
        }

        public void Tick(float d)
        {
            this.PuzzleDiscussionWindowsContainer.Tick(d);
        }

        public void PlayDiscussion(DiscussionTreeId DiscussionTreeId, Dictionary<CutsceneParametersName, object> discussionGraphParameters)
        {
            this.PuzzleDiscussionWindowsContainer.AddDiscussionTree(new PuzzleDiscussionWindowManager(DiscussionTreeId, discussionGraphParameters));
        }

    }
}
