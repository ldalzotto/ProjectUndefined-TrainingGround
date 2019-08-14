using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace CoreGame
{
    [System.Serializable]
    public class TutorialTextAction : SequencedAction
    {
        [CustomEnum()]
        [SerializeField]
        public DiscussionTextID DiscussionTextID;

        [NonSerialized]
        private DiscussionWindow DiscussionWindow;

        [NonSerialized]
        private bool discussionEnded = false;
        [NonSerialized]
        private TutorialActionInput TutorialActionInput;

        public TutorialTextAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.discussionEnded;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.discussionEnded = false;
            this.playerCrossedDistance = 0f;

            this.TutorialActionInput = (TutorialActionInput)ContextActionInput;
            
            this.DiscussionWindow = DiscussionWindow.Instanciate(this.TutorialActionInput.MainCanvas);
            this.DiscussionWindow.InitializeDependencies(() => { this.discussionEnded = true; });
            this.DiscussionWindow.OnDiscussionWindowAwake(this.TutorialActionInput.DiscussionTextConfiguration.ConfigurationInherentData[this.DiscussionTextID], this.TutorialActionInput.DiscussionPositionManager.GetDiscussionPosition(DiscussionPositionMarkerID.TOP_LEFT).transform);
        }

        private float playerCrossedDistance = 0f;
        private Nullable<Vector3> lastFramePlayerPosition;
    
        public override void Tick(float d)
        {
            this.DiscussionWindow.Tick(d);

            if(this.playerCrossedDistance >= 0f)
            {
                //TODO -> move to it's own piece of code
                var currentPlayerPosition = this.TutorialActionInput.PlayerManagerType.transform.position;
                if (lastFramePlayerPosition.HasValue)
                {
                    this.playerCrossedDistance += Vector3.Distance(this.lastFramePlayerPosition.Value, currentPlayerPosition);
                }
                this.lastFramePlayerPosition = currentPlayerPosition;

                if (this.playerCrossedDistance >= 20)
                {
                    Debug.Log("CLOSE");
                    this.playerCrossedDistance = -1;
                    this.DiscussionWindow.PlayDiscussionCloseAnimation();
                }
            }
      

        }

        public override void Interupt()
        {
            base.Interupt();
            this.DiscussionWindow.PlayDiscussionCloseAnimation();
            this.discussionEnded = true;
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.DiscussionTextID = (DiscussionTextID)NodeEditorGUILayout.EnumField("Discussion Text : ", string.Empty, this.DiscussionTextID);
        }
#endif
    }

}
