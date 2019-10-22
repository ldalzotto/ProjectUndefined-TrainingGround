using System;
using System.Collections.Generic;
using CoreGame;
using GameConfigurationID;
using PlayerObject;
using UnityEngine;

namespace Tutorial
{
    public class MovementTutorialTextAction : AbstractTutorialTextAction
    {
        private DiscussionPositionMarkerID DiscussionPositionMarkerID;

        public override void AfterFinishedEventProcessed()
        {
        }

        public MovementTutorialTextAction(DiscussionTextID DiscussionTextID, DiscussionPositionMarkerID DiscussionPositionMarkerID, List<SequencedAction> nextActions) : base(DiscussionTextID, nextActions)
        {
            this.DiscussionPositionMarkerID = DiscussionPositionMarkerID;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            #region External Dependencies

            var discussionTextConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.DiscussionTextConfiguration;

            #endregion

            base.FirstExecutionAction(ContextActionInput);
            this.DiscussionWindow.OnDiscussionWindowAwakeV2(discussionTextConfiguration.ConfigurationInherentData[this.DiscussionTextID],
                DiscussionPositionManager.Get().GetDiscussionPosition(DiscussionPositionMarkerID).transform.position, WindowPositionType.SCREEN);
        }

        protected override ITutorialTextActionManager GetTutorialTextManager()
        {
            return new MovementTutorialTextActionmanager();
        }
    }

    class MovementTutorialTextActionmanager : ITutorialTextActionManager
    {
        private float playerCrossedDistance = 0f;
        private Nullable<Vector3> lastFramePlayerPosition;

        private PlayerInteractiveObject PlayerInteractiveObject;

        public void FirstExecutionAction(DiscussionTextID DiscussionTextID, DiscussionWindow discussionWindow)
        {
            this.playerCrossedDistance = 0f;
            this.lastFramePlayerPosition = null;
            this.PlayerInteractiveObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;
        }

        public bool Tick(float d)
        {
            if (this.playerCrossedDistance >= 0f)
            {
                var currentPlayerPosition = this.PlayerInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition;
                if (lastFramePlayerPosition.HasValue)
                {
                    this.playerCrossedDistance += Vector3.Distance(this.lastFramePlayerPosition.Value, currentPlayerPosition);
                }

                this.lastFramePlayerPosition = currentPlayerPosition;

                if (this.playerCrossedDistance >= 20)
                {
                    this.playerCrossedDistance = -1;
                    return true;
                }
            }

            return false;
        }
    }
}