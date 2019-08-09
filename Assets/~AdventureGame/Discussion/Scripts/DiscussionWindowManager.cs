using CoreGame;
using GameConfigurationID;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public class DiscussionWindowManager
    {
        #region External Dependencies
        private Canvas GameCanvas;
        private CoreStaticConfiguration CoreStaticConfiguration;
        private AdventureGameConfigurationManager AdventureGameConfigurationManager;
        private PointOfInterestManager PointOfInterestManager;
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

        public DiscussionWindowManager(DiscussionTreeId DiscussionTreeId)
        {
            #region External Dependencies
            GameCanvas = GameObject.FindObjectOfType<Canvas>();
            this.CoreStaticConfiguration = GameObject.FindObjectOfType<CoreStaticConfigurationContainer>().CoreStaticConfiguration;
            this.AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
            this.PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            #endregion

            this.discussionTreeID = DiscussionTreeId;
            DicussionInputManager = new DicussionInputManager(GameInputManager);
            
            this.OpenedDiscussion = MonoBehaviour.Instantiate(CoreGame.PrefabContainer.Instance.DiscussionUIPrefab, GameCanvas.transform, false);
            this.DiscussionTreePlayer = new DiscussionTreePlayer(this.AdventureGameConfigurationManager.DiscussionTreeConf()[DiscussionTreeId],
                            this.OnDiscussionTextWindowAwake,
                            this.OnChoicePopupAwake);

            this.DiscussionTreePlayer.StartDiscussion();
        }

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

        private void OnDiscussionTextWindowAwake(DiscussionTextOnlyNode discussionNode)
        {
            OpenedDiscussion.gameObject.SetActive(true);
            OpenedDiscussion.transform.localScale = Vector3.zero;
            OpenedDiscussion.InitializeDependencies();
            OpenedDiscussion.OnDiscussionWindowAwake(discussionNode, this.PointOfInterestManager.GetActivePointOfInterest(discussionNode.Talker).transform, ref this.CoreStaticConfiguration.DiscussionTestRepertoire);
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