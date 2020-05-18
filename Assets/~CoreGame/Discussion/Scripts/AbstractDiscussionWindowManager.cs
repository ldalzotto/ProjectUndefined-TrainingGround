﻿using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public abstract class AbstractDiscussionWindowManager
    {
        #region External Dependencies
        private Canvas GameCanvas;
        private CoreConfigurationManager CoreConfigurationManager;
        protected DiscussionPositionManager DiscussionPositionManager;
        #endregion

        #region State 
        private DiscussionWindowManagerStrategy DiscussionWindowManagerStrategy;
        private ChoicePopup OpenedChoicePopup;
        #endregion

        private DiscussionTreeId discussionTreeID;
        private DicussionInputManager DicussionInputManager;
        private DiscussionWindow OpenedDiscussion;
        private DiscussionTreePlayer DiscussionTreePlayer;

        #region Logical Statements
        public bool IsDiscussionFinished()
        {
            return this.DiscussionTreePlayer.IsConversationFinished;
        }
        private bool IsWaitForInputStrategyEnabled()
        {
            return this.DiscussionWindowManagerStrategy == null || this.DiscussionWindowManagerStrategy.GetType() == typeof(WaitForInputDiscussionWindowManagerStrategy);
        }
        private bool IsWaitForSecondsStrategyEnabled()
        {
            return this.DiscussionWindowManagerStrategy != null && this.DiscussionWindowManagerStrategy.GetType() == typeof(WaitForSecondsDiscussionWindowManagerStrategy);
        }
        private bool IsWaitForSecondsStrategyTimeOver()
        {
            return this.IsWaitForSecondsStrategyEnabled() && ((WaitForSecondsDiscussionWindowManagerStrategy)this.DiscussionWindowManagerStrategy).IsTimeOver();
        }
        #endregion

        #region Data Retrieval
        public DiscussionTreeId DiscussionTreeID { get => discussionTreeID; }
        #endregion

        protected void BaseInit(DiscussionTreeId DiscussionTreeId, DiscussionWindowManagerStrategy DiscussionWindowManagerStrategy)
        {
            #region External Dependencies
            GameCanvas = GameObject.FindObjectOfType<Canvas>();
            this.CoreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();
            this.DiscussionPositionManager = GameObject.FindObjectOfType<DiscussionPositionManager>();

            var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            #endregion

            this.discussionTreeID = DiscussionTreeId;
            this.DiscussionWindowManagerStrategy = DiscussionWindowManagerStrategy;
            DicussionInputManager = new DicussionInputManager(GameInputManager);

            this.OpenedDiscussion = MonoBehaviour.Instantiate(CoreGame.PrefabContainer.Instance.DiscussionUIPrefab, GameCanvas.transform, false);

            this.DiscussionTreePlayer = new DiscussionTreePlayer(this.CoreConfigurationManager.DiscussionConfiguration()[DiscussionTreeId],
                            this.OnDiscussionTextWindowAwake,
                            this.OnChoicePopupAwake);

            this.DiscussionTreePlayer.StartDiscussion();
        }

        protected virtual Transform GetAbstractTextOnlyNodePosition(AbstractDiscussionTextOnlyNode abstractDiscussionTextOnlyNode)
        {
            if (abstractDiscussionTextOnlyNode.GetType() == typeof(FixedScreenPositionDiscussionTextOnlyNode))
            {
                return this.DiscussionPositionManager.GetDiscussionPosition(((FixedScreenPositionDiscussionTextOnlyNode)abstractDiscussionTextOnlyNode).DiscussionScreenPosition).transform;
            }
            return null;
        }

        public void Tick(float d, out Nullable<DiscussionNodeId> discussionChoiceMade)
        {
            discussionChoiceMade = null;
            if (this.IsWaitForInputStrategyEnabled())
            {
                TickWaitForInput(d, out discussionChoiceMade);
            }
            else if (this.IsWaitForSecondsStrategyEnabled())
            {
                TickWaitForSeconds(d, out discussionChoiceMade);
            }
        }

        private void TickWaitForInput(float d, out DiscussionNodeId? discussionChoiceMade)
        {
            discussionChoiceMade = null;
            if (OpenedChoicePopup != null)
            {
                OpenedChoicePopup.Tick(d);
                if (DicussionInputManager.Tick())
                {
                    discussionChoiceMade = OnChoiceMade();
                }
            }
            else
            {
                OpenedDiscussion.Tick(d);
                if (!OpenedDiscussion.IsWriting())
                {
                    if (DicussionInputManager.Tick())
                    {
                        if (OpenedDiscussion.IsWaitingForCloseInput())
                        {
                            if (this.DiscussionTreePlayer.OnDiscussionTextNodeEnd())
                            {
                                this.OpenedDiscussion.PlayDiscussionCloseAnimation();
                            }
                        }
                        else if (OpenedDiscussion.IsWaitingForContinueInput())
                        {
                            OpenedDiscussion.ProcessDiscussionContinue();
                        }
                    }
                }
            }
        }

        private DiscussionNodeId? OnChoiceMade()
        {
            DiscussionNodeId? discussionChoiceMade;
            var selectedChoice = OpenedChoicePopup.GetSelectedDiscussionChoice();
            MonoBehaviour.Destroy(OpenedChoicePopup.gameObject);
            discussionChoiceMade = selectedChoice.DiscussionNodeId;
            this.DiscussionTreePlayer.OnDiscussionChoiceMade(selectedChoice.DiscussionNodeId);
            this.OpenedDiscussion.PlayDiscussionCloseAnimation();
            return discussionChoiceMade;
        }

        private void TickWaitForSeconds(float d, out DiscussionNodeId? discussionChoiceMade)
        {
            discussionChoiceMade = null;
            var WaitForSecondsDiscussionWindowManagerStrategy = (WaitForSecondsDiscussionWindowManagerStrategy)DiscussionWindowManagerStrategy;

            if (WaitForSecondsDiscussionWindowManagerStrategy.IsTimeOver() && !this.OpenedDiscussion.IsExitAnimationPlaying())
            {
                this.OpenedDiscussion.PlayDiscussionCloseAnimation();
            }
            else
            {
                OpenedDiscussion.Tick(d);

                if (!OpenedDiscussion.IsWriting())
                {
                    if (OpenedDiscussion.IsWaitingForCloseInput())
                    {
                        if (this.DiscussionTreePlayer.OnDiscussionTextNodeEnd())
                        {
                            this.OpenedDiscussion.PlayDiscussionCloseAnimation();
                        }
                    }
                    else if (OpenedDiscussion.IsWaitingForContinueInput())
                    {
                        OpenedDiscussion.ProcessDiscussionContinue();
                    }
                }
            }

            WaitForSecondsDiscussionWindowManagerStrategy.ElapsedTime += d;
        }

        #region External Event
        public void OnDiscussionEnded()
        {
            MonoBehaviour.Destroy(this.OpenedDiscussion.gameObject);
        }
        #endregion

        private void OnDiscussionTextWindowAwake(AbstractDiscussionTextOnlyNode discussionNode)
        {
            OpenedDiscussion.gameObject.SetActive(true);
            OpenedDiscussion.transform.localScale = Vector3.zero;
            OpenedDiscussion.InitializeDependencies(this.OnDiscussionWindowAnimationExitFinished);
            OpenedDiscussion.OnDiscussionWindowAwake(this.CoreConfigurationManager.DiscussionTextConfigurationData()[discussionNode.DisplayedText].Text, this.GetAbstractTextOnlyNodePosition(discussionNode));
        }

        private void OnChoicePopupAwake(List<DiscussionChoice> nexDiscussionChoices)
        {
            OpenedChoicePopup = MonoBehaviour.Instantiate(CoreGame.PrefabContainer.Instance.ChoicePopupPrefab, OpenedDiscussion.transform);
            OpenedChoicePopup.OnChoicePopupAwake(nexDiscussionChoices, Vector2.zero, this.CoreConfigurationManager.DiscussionTextConfiguration());
        }

        private void OnDiscussionWindowAnimationExitFinished()
        {
            if (this.IsWaitForSecondsStrategyTimeOver())
            {
                this.DiscussionTreePlayer.AbortDiscussionTree();
            }
            else
            {
                this.OnDiscussionWindowSleep();
                this.DiscussionTreePlayer.OnDiscussionNodeFinished();
            }
        }

        private void OnDiscussionWindowSleep()
        {
            OpenedDiscussion.OnDiscussionWindowSleep();

            if (OpenedChoicePopup != null)
            {
                OpenedChoicePopup = null;
            }

            OpenedDiscussion.gameObject.SetActive(false);
        }
    }

    public abstract class DiscussionWindowManagerStrategy { }
    public class WaitForInputDiscussionWindowManagerStrategy : DiscussionWindowManagerStrategy { }
    public class WaitForSecondsDiscussionWindowManagerStrategy : DiscussionWindowManagerStrategy
    {
        public float SecondsToWait;
        public float ElapsedTime;

        public WaitForSecondsDiscussionWindowManagerStrategy(float secondsToWait)
        {
            SecondsToWait = secondsToWait;
            ElapsedTime = 0f;
        }

        public bool IsTimeOver() { return this.ElapsedTime >= this.SecondsToWait; }
    }

    #region Discussion Input Handling
    class DicussionInputManager
    {
        private GameInputManager GameInputManager;

        public DicussionInputManager(GameInputManager gameInputManager)
        {
            GameInputManager = gameInputManager;
        }

        public bool Tick()
        {
            return GameInputManager.CurrentInput.ActionButtonD();
        }
    }
    #endregion
}
