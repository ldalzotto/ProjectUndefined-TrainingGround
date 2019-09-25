using System;
using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{
    public class DiscussionWindowsContainer : AbstractDiscussionWindowsContainer
    {

        #region External Dependencies
        private DiscussionEventHandler DiscussionEventHandler;
        #endregion

        public override void Init()
        {
            base.Init();
            this.DiscussionEventHandler = AdventureGameSingletonInstances.DiscussionEventHandler;
        }

        protected override void OnChoiceMade(DiscussionNodeId choice)
        {
            DiscussionEventHandler.OnDiscussionChoiceMade(choice);
        }
    }
}
