using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class DiscussionWindowsContainer : MonoBehaviour
    {

        #region External Dependencies
        private DiscussionEventHandler DiscussionEventHandler;
        #endregion

        private Dictionary<DiscussionTreeId, DiscussionWindowManager> discussionWindowsManager;

        public void Init()
        {
            this.discussionWindowsManager = new Dictionary<DiscussionTreeId, DiscussionWindowManager>();
            this.DiscussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
        }

        public void Tick(float d)
        {
            List<DiscussionWindowManager> discussionWindowManagersThatEnded = null;
            foreach (var discussionWindowManager in this.discussionWindowsManager.Values)
            {
                discussionWindowManager.Tick(d, out Nullable<DiscussionNodeId> discussionChoiceMade);

                if(discussionChoiceMade != null && discussionChoiceMade.HasValue)
                {
                    DiscussionEventHandler.OnDiscussionChoiceMade(discussionChoiceMade.Value);
                }

                if (discussionWindowManager.IsDiscussionFinished())
                {
                    if (discussionWindowManagersThatEnded == null)
                    {
                        discussionWindowManagersThatEnded = new List<DiscussionWindowManager>();
                    }
                }
            }

            if (discussionWindowManagersThatEnded != null)
            {
                foreach(var discussionWindowManagerThatEnded in discussionWindowManagersThatEnded)
                {
                    this.DiscussionEventHandler.OnDiscussionTreeEnd(discussionWindowManagerThatEnded.DiscussionTreeID);
                }
            }
        }

        public void GUITick()
        {
            foreach (var discussionWindowManager in this.discussionWindowsManager.Values)
            {
                discussionWindowManager.GUITick();
            }
        }

        #region External Events
        public DiscussionWindowManager OnDiscussionTreeStart(DiscussionTreeId discussionTreeId)
        {
            this.discussionWindowsManager[discussionTreeId] = new DiscussionWindowManager(discussionTreeId);
            return this.discussionWindowsManager[discussionTreeId];
        }
        public void OnDiscussionTreeEnd(DiscussionTreeId discussionTreeId)
        {
            this.discussionWindowsManager[discussionTreeId].OnDiscussionEnded();
            this.discussionWindowsManager.Remove(discussionTreeId);
        }
        #endregion
    }
}
