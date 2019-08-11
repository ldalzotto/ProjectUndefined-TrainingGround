using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameConfigurationID;
using System;

namespace CoreGame
{
    public abstract class AbstractDiscussionWindowsContainer : MonoBehaviour
    {

        protected Dictionary<DiscussionTreeId, AbstractDiscussionWindowManager> discussionWindowsManager;

        public virtual void Init()
        {
            this.discussionWindowsManager = new Dictionary<DiscussionTreeId, AbstractDiscussionWindowManager>();
        }

        public virtual void Tick(float d)
        {
            List<AbstractDiscussionWindowManager> discussionWindowManagersThatEnded = null;
            foreach (var discussionWindowManager in this.discussionWindowsManager.Values)
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
                    this.RemoveDiscussionTree(discussionWindowManagerThatEnded.DiscussionTreeID);
                }
            }
        }

        protected virtual void OnChoiceMade(DiscussionNodeId choice) { }
        protected virtual void OnDiscussionTreeEnd(DiscussionTreeId discussionTreeId) { }
        
        private void RemoveDiscussionTree(DiscussionTreeId discussionTreeId)
        {
            this.OnDiscussionTreeEnd(discussionTreeId);
            this.discussionWindowsManager[discussionTreeId].OnDiscussionEnded();
            this.discussionWindowsManager.Remove(discussionTreeId);
        }
    }
}
