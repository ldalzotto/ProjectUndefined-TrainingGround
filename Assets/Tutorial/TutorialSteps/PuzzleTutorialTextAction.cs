using System.Collections.Generic;
using CoreGame;
using GameConfigurationID;
using PlayerActions_Interfaces;

namespace Tutorial
{
    public class ActionWheelTutorialStepAction : AbstractTutorialTextAction
    {
        private DiscussionPositionMarkerID DiscussionPositionMarkerID;
        private PuzzlePlayerActionWheelOpenTutorialTextActionManager PuzzlePlayerActionWheelOpenTutorialTextActionManager;

        public ActionWheelTutorialStepAction(DiscussionTextID DiscussionTextID, DiscussionPositionMarkerID DiscussionPositionMarkerID, List<SequencedAction> nextActions) : base(DiscussionTextID, nextActions)
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
            this.PuzzlePlayerActionWheelOpenTutorialTextActionManager = new PuzzlePlayerActionWheelOpenTutorialTextActionManager();
            return this.PuzzlePlayerActionWheelOpenTutorialTextActionManager;
        }

        public override void AfterFinishedEventProcessed()
        {
            this.PuzzlePlayerActionWheelOpenTutorialTextActionManager.Destroy();
        }

        public override void Interupt()
        {
            base.Interupt();
            this.PuzzlePlayerActionWheelOpenTutorialTextActionManager.Destroy();
        }
    }

    internal class PuzzlePlayerActionWheelOpenTutorialTextActionManager : ITutorialTextActionManager
    {
        private bool isPlayerActionWheelAwaken;

        public void FirstExecutionAction(DiscussionTextID DiscussionTextID, DiscussionWindow discussionWindow)
        {
            PlayerActionsEventListenerManager.Get().RegisterOnPlayerActionSelectionWheelAwakeEventListener(this.OnPlayerActionWheelAwake);
        }

        public bool Tick(float d)
        {
            return isPlayerActionWheelAwaken;
        }

        public void OnPlayerActionWheelAwake()
        {
            isPlayerActionWheelAwaken = true;
        }

        public void Destroy()
        {
            PlayerActionsEventListenerManager.Get().UnRegisterOnPlayerActionSelectionWheelAwakeEventListener(this.OnPlayerActionWheelAwake);
        }
    }
}