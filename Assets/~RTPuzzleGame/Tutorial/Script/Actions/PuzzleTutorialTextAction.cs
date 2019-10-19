using System;
using System.Collections.Generic;
using CoreGame;
using GameConfigurationID;
#if UNITY_EDITOR
using NodeGraph_Editor;

#endif

namespace RTPuzzle
{
    [Serializable]
    public class PuzzleTutorialTextAction : AbstractTutorialTextAction, ITutorialEventListener
    {
        public PuzzleTutorialTextAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        protected override ITutorialTextActionManager GetTutorialTextManager(TutorialActionInput tutorialActionInput)
        {
            if (tutorialActionInput.TutorialStepID == TutorialStepID.PUZZLE_CONTEXT_ACTION_AWAKE) return new PuzzlePlayerActionWheelOpenTutorialTextActionManager();

            return null;
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            DiscussionTextID = (DiscussionTextID) NodeEditorGUILayout.EnumField("Discussion Text : ", string.Empty, DiscussionTextID);
        }
#endif

        #region External Events

        public void OnPlayerActionWheelAwake()
        {
            if (TutorialActionInput.TutorialStepID == TutorialStepID.PUZZLE_CONTEXT_ACTION_AWAKE) ((PuzzlePlayerActionWheelOpenTutorialTextActionManager) TutorialTextActionManager).OnPlayerActionWheelAwake();
        }

        public void OnPlayerActionWheelSleep()
        {
        }

        public void OnPlayerActionWheelNodeSelected()
        {
        }

        #endregion
    }

    internal class PuzzlePlayerActionWheelOpenTutorialTextActionManager : ITutorialTextActionManager
    {
        private bool isPlayerActionWheelAwaken;

        public void FirstExecutionAction(TutorialActionInput TutorialActionInput, DiscussionTextID DiscussionTextID, DiscussionWindow discussionWindow)
        {
            isPlayerActionWheelAwaken = false;
            discussionWindow.OnDiscussionWindowAwakeV2(TutorialActionInput.DiscussionTextConfiguration.ConfigurationInherentData[DiscussionTextID],
                TutorialActionInput.DiscussionPositionManager.GetDiscussionPosition(DiscussionPositionMarkerID.TOP_LEFT).transform.position, WindowPositionType.SCREEN);
        }

        public bool Tick(float d)
        {
            return isPlayerActionWheelAwaken;
        }

        public void OnPlayerActionWheelAwake()
        {
            isPlayerActionWheelAwaken = true;
        }
    }
}