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
    public class PuzzleTutorialTextAction : AbstractTutorialTextAction
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
            return null;
        }

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
            this.TimeFlowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            discussionWindow.OnDiscussionWindowAwakeV2(TutorialActionInput.DiscussionTextConfiguration.ConfigurationInherentData[DiscussionTextID],
                                GameObject.FindObjectOfType<TimeFlowBarManager>().transform.position + new Vector3(0, Screen.height * 0.05f), WindowPositionType.SCREEN);
        }

        public bool Tick(float d)
        {
            if (this.lastFrameTimeFlowCurrentTime != -1f)
            {
                this.puzzleTimeElapsed += Mathf.Abs(this.TimeFlowManager.GetCurrentAvailableTime() - this.lastFrameTimeFlowCurrentTime);
                if(this.puzzleTimeElapsed >= 1f)
                {
                    return true;
                }
            }
            this.lastFrameTimeFlowCurrentTime = this.TimeFlowManager.GetCurrentAvailableTime();
            return false;
        }
    }

}
