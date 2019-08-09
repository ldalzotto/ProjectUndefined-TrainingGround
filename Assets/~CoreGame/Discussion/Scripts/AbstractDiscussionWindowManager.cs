using GameConfigurationID;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public abstract class AbstractDiscussionWindowManager
    {
        #region External Dependencies
        private Canvas GameCanvas;
        private CoreStaticConfiguration CoreStaticConfiguration;
        private CoreConfigurationManager CoreConfigurationManager;
        #endregion

        private DiscussionTreeId discussionTreeID;
        private DicussionInputManager DicussionInputManager;
        private DiscussionWindow OpenedDiscussion;
        private ChoicePopup OpenedChoicePopup;
        private DiscussionTreePlayer DiscussionTreePlayer;

        #region Logical Statements
        public bool IsDiscussionFinished()
        {
            return this.DiscussionTreePlayer.IsConversationFinished;
        }
        #endregion

        #region Data Retrieval
        public DiscussionTreeId DiscussionTreeID { get => discussionTreeID; }
        #endregion

        protected void BaseInit(DiscussionTreeId DiscussionTreeId)
        {
            #region External Dependencies
            GameCanvas = GameObject.FindObjectOfType<Canvas>();
            this.CoreStaticConfiguration = GameObject.FindObjectOfType<CoreStaticConfigurationContainer>().CoreStaticConfiguration;
            this.CoreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();

            var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            #endregion

            this.discussionTreeID = DiscussionTreeId;
            DicussionInputManager = new DicussionInputManager(GameInputManager);

            this.OpenedDiscussion = MonoBehaviour.Instantiate(CoreGame.PrefabContainer.Instance.DiscussionUIPrefab, GameCanvas.transform, false);
            this.DiscussionTreePlayer = new DiscussionTreePlayer(this.CoreConfigurationManager.DiscussionConfiguration()[DiscussionTreeId],
                            this.OnDiscussionTextWindowAwake,
                            this.OnChoicePopupAwake);

            this.DiscussionTreePlayer.StartDiscussion();
        }

        protected abstract Transform GetAbstractTextOnlyNodePosition(AbstractDiscussionTextOnlyNode abstractDiscussionTextOnlyNode);

        public void Tick(float d, out Nullable<DiscussionNodeId> discussionChoiceMade)
        {
            discussionChoiceMade = null;
            if (!OpenedDiscussion.IsExitAnimationPlaying())
            {
                if (OpenedChoicePopup != null)
                {
                    OpenedChoicePopup.Tick(d);
                    if (DicussionInputManager.Tick())
                    {
                        var selectedChoice = OpenedChoicePopup.GetSelectedDiscussionChoice();
                        MonoBehaviour.Destroy(OpenedChoicePopup.gameObject);
                        discussionChoiceMade = selectedChoice.DiscussionNodeId;
                        this.DiscussionTreePlayer.OnDiscussionChoiceMade(selectedChoice.DiscussionNodeId);
                        Coroutiner.Instance.StartCoroutine(this.OnDiscussionWindowSleepCoRoutine());
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
                                    Coroutiner.Instance.StartCoroutine(this.OnDiscussionWindowSleepCoRoutine());
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
        }

        public void GUITick()
        {
            if (OpenedDiscussion != null)
            {
                OpenedDiscussion.OnGUIDraw();
            }
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
            OpenedDiscussion.InitializeDependencies();
            OpenedDiscussion.OnDiscussionWindowAwake(discussionNode, this.GetAbstractTextOnlyNodePosition(discussionNode), ref this.CoreStaticConfiguration.DiscussionTestRepertoire);
        }

        private void OnChoicePopupAwake(List<DiscussionChoice> nexDiscussionChoices)
        {
            OpenedChoicePopup = MonoBehaviour.Instantiate(CoreGame.PrefabContainer.Instance.ChoicePopupPrefab, OpenedDiscussion.transform);
            OpenedChoicePopup.OnChoicePopupAwake(nexDiscussionChoices, Vector2.zero, ref this.CoreStaticConfiguration.DiscussionTestRepertoire);
        }

        private IEnumerator OnDiscussionWindowSleepCoRoutine()
        {
            yield return OpenedDiscussion.PlayDiscussionCloseAnimation();
            this.OnDiscussionWindowSleep();
            this.DiscussionTreePlayer.OnDiscussionNodeFinished();
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
