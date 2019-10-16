using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    public class PuzzleTutorialTextAction : AbstractTutorialTextAction, ITutorialEventListener
    {

        public override void AfterFinishedEventProcessed()
        {
        }

        public PuzzleTutorialTextAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        protected override ITutorialTextActionManager GetTutorialTextManager(TutorialActionInput tutorialActionInput)
        {
            if (tutorialActionInput.TutorialStepID == TutorialStepID.PUZZLE_TIME_ELAPSING)
            {
                return new PuzzleTimeElapsingTutorialTextActionmanager();
            }
            else if (tutorialActionInput.TutorialStepID == TutorialStepID.PUZZLE_CONTEXT_ACTION_AWAKE)
            {
                return new PuzzlePlayerActionWheelOpenTutorialTextActionManager();
            }
            return null;
        }

        #region External Events

        public void OnPlayerActionWheelAwake()
        {
            if (TutorialActionInput.TutorialStepID == TutorialStepID.PUZZLE_CONTEXT_ACTION_AWAKE)
            {
                ((PuzzlePlayerActionWheelOpenTutorialTextActionManager)this.TutorialTextActionManager).OnPlayerActionWheelAwake();
            }
        }

        public void OnPlayerActionWheelSleep() { }

        public void OnPlayerActionWheelNodeSelected() { }

        #endregion

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.DiscussionTextID = (DiscussionTextID)NodeEditorGUILayout.EnumField("Discussion Text : ", string.Empty, this.DiscussionTextID);
        }
#endif
    }

    class PuzzleTimeElapsingTutorialTextActionmanager : ITutorialTextActionManager
    {
        private TimeFlowManager TimeFlowManager;

        private float puzzleTimeElapsed;
        private float lastFrameTimeFlowCurrentTime;

        public void FirstExecutionAction(TutorialActionInput TutorialActionInput, DiscussionTextID DiscussionTextID, DiscussionWindow discussionWindow)
        {
            this.puzzleTimeElapsed = 0f;
            this.lastFrameTimeFlowCurrentTime = -1f;
            this.TimeFlowManager = TimeFlowManager.Get();
            discussionWindow.OnDiscussionWindowAwakeV2(TutorialActionInput.DiscussionTextConfiguration.ConfigurationInherentData[DiscussionTextID],
                                TimeFlowBarManager.Get().GetScreenPosition() + new Vector3(0, Screen.height * 0.05f), WindowPositionType.SCREEN);
        }

        public bool Tick(float d)
        {
            if (this.lastFrameTimeFlowCurrentTime != -1f)
            {
                this.puzzleTimeElapsed += Mathf.Abs(this.TimeFlowManager.GetCurrentAvailableTime() - this.lastFrameTimeFlowCurrentTime);
                if (this.puzzleTimeElapsed >= 1f)
                {
                    return true;
                }
            }
            this.lastFrameTimeFlowCurrentTime = this.TimeFlowManager.GetCurrentAvailableTime();
            return false;
        }
    }

    class PuzzlePlayerActionWheelOpenTutorialTextActionManager : ITutorialTextActionManager
    {
        private bool isPlayerActionWheelAwaken;

        public void FirstExecutionAction(TutorialActionInput TutorialActionInput, DiscussionTextID DiscussionTextID, DiscussionWindow discussionWindow)
        {
            this.isPlayerActionWheelAwaken = false;
            discussionWindow.OnDiscussionWindowAwakeV2(TutorialActionInput.DiscussionTextConfiguration.ConfigurationInherentData[DiscussionTextID],
                 TutorialActionInput.DiscussionPositionManager.GetDiscussionPosition(DiscussionPositionMarkerID.TOP_LEFT).transform.position, WindowPositionType.SCREEN);
        }

        public bool Tick(float d)
        {
            return this.isPlayerActionWheelAwaken;
        }

        public void OnPlayerActionWheelAwake()
        {
            this.isPlayerActionWheelAwaken = true;
        }
    }

}
