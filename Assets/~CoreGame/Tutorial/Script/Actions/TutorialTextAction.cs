using System;
using System.Collections.Generic;
using GameConfigurationID;
using UnityEngine;
#if UNITY_EDITOR
using NodeGraph_Editor;

#endif

namespace CoreGame
{
    [Serializable]
    public class TutorialTextAction : AbstractTutorialTextAction
    {
        [CustomEnum()] [SerializeField] public DiscussionPositionMarkerID DiscussionPositionMarkerID;

        public override void AfterFinishedEventProcessed()
        {
        }

        public TutorialTextAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            base.FirstExecutionAction(ContextActionInput);
            this.DiscussionWindow.OnDiscussionWindowAwakeV2(this.TutorialActionInput.DiscussionTextConfiguration.ConfigurationInherentData[this.DiscussionTextID],
                this.TutorialActionInput.DiscussionPositionManager.GetDiscussionPosition(DiscussionPositionMarkerID).transform.position, WindowPositionType.SCREEN);
        }

        protected override ITutorialTextActionManager GetTutorialTextManager(TutorialActionInput tutorialActionInput)
        {
            if (tutorialActionInput.TutorialStepID == TutorialStepID.TUTORIAL_MOVEMENT)
            {
                return new MovementTutorialTextActionmanager();
            }

            return null;
        }


        public override void Tick(float d)
        {
            base.Tick(d);
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.DiscussionTextID = (DiscussionTextID) NodeEditorGUILayout.EnumField("Discussion Text : ", string.Empty, this.DiscussionTextID);
            this.DiscussionPositionMarkerID = (DiscussionPositionMarkerID) NodeEditorGUILayout.EnumField("Discussion Position : ", string.Empty, this.DiscussionPositionMarkerID);
        }
#endif
    }

    class MovementTutorialTextActionmanager : ITutorialTextActionManager
    {
        private float playerCrossedDistance = 0f;
        private Nullable<Vector3> lastFramePlayerPosition;

        //  private PlayerManagerType PlayerManagerType;

        public void FirstExecutionAction(TutorialActionInput TutorialActionInput, DiscussionTextID DiscussionTextID, DiscussionWindow discussionWindow)
        {
            this.playerCrossedDistance = 0f;
            this.lastFramePlayerPosition = null;
            // this.PlayerManagerType = TutorialActionInput.PlayerManagerType;
        }

        public bool Tick(float d)
        {
            if (this.playerCrossedDistance >= 0f)
            {
                //TODO -> When having a tutorial module, add dependency to player
                var currentPlayerPosition = Vector3.zero;
                //this.PlayerManagerType.transform.position;
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