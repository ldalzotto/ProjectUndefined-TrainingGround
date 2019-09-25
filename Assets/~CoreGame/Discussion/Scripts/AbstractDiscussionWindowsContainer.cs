using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameConfigurationID;
using System;

namespace CoreGame
{
    public abstract class AbstractDiscussionWindowsContainer : MonoBehaviour
    {
        private List<AbstractDiscussionWindowManager> discussionWindowsManagers;

        public virtual void Init()
        {
            this.discussionWindowsManagers = new List<AbstractDiscussionWindowManager>();
        }

        public virtual void Tick(float d)
        {
            List<AbstractDiscussionWindowManager> discussionWindowManagersThatEnded = null;
            foreach (var discussionWindowManager in this.discussionWindowsManagers)
            {
                discussionWindowManager.Tick(d, out Nullable<DiscussionNodeId> discussionChoiceMade);

                if (discussionChoiceMade != null && discussionChoiceMade.HasValue)
                {
                    this.OnChoiceMade(discussionChoiceMade.Value);
                }

                if (discussionWindowManager.IsDiscussionFinished())
                {
                    if (discussionWindowManagersThatEnded == null)
                    {
                        discussionWindowManagersThatEnded = new List<AbstractDiscussionWindowManager>();
                        discussionWindowManagersThatEnded.Add(discussionWindowManager);
                    }
                }
            }

            if (discussionWindowManagersThatEnded != null)
            {
                foreach (var discussionWindowManagerThatEnded in discussionWindowManagersThatEnded)
                {
                    this.RemoveDiscussionTree(discussionWindowManagerThatEnded);
                }
            }
        }

        protected virtual void OnChoiceMade(DiscussionNodeId choice) { }
        protected virtual void OnDiscussionTreeEnd(DiscussionTreeId discussionTreeId) { }
        
        public void AddDiscussionTree(AbstractDiscussionWindowManager discussionWindowManager)
        {
            this.discussionWindowsManagers.Add(discussionWindowManager);
        }

        private void RemoveDiscussionTree(AbstractDiscussionWindowManager discussionWindowManager)
        {
            this.OnDiscussionTreeEnd(discussionWindowManager.DiscussionTreeID);
            this.discussionWindowsManagers.Remove(discussionWindowManager);
        }
    }
}
