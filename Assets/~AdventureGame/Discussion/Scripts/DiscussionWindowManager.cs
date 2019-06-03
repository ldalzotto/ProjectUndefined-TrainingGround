using CoreGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public class DiscussionWindowManager : MonoBehaviour
    {
        #region External Dependencies
        private Canvas GameCanvas;
        private AdventureStaticConfiguration AdventureStaticConfiguration;
        #endregion

        private DicussionInputManager DicussionInputManager;
        private DiscussionWindow OpenedDiscussion;
        private ChoicePopup OpenedChoicePopup;
        private DiscussionEventHandler DiscussionEventHandler;

        private void Start()
        {
            #region External Dependencies
            GameCanvas = GameObject.FindObjectOfType<Canvas>();
            this.AdventureStaticConfiguration = GameObject.FindObjectOfType<AdventureStaticConfigurationContainer>().AdventureStaticConfiguration;
            var GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            #endregion

            DicussionInputManager = new DicussionInputManager(GameInputManager);
            DiscussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
        }

        public void Tick(float d)
        {
            if (OpenedDiscussion != null && !OpenedDiscussion.IsExitAnimationPlaying())
            {
                if (OpenedChoicePopup != null)
                {
                    OpenedChoicePopup.Tick(d);
                    if (DicussionInputManager.Tick())
                    {
                        var selectedChoice = OpenedChoicePopup.GetSelectedDiscussionChoice();
                        DiscussionEventHandler.OnDiscussionChoiceEnd(selectedChoice.DiscussionNodeId);
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
                                DiscussionEventHandler.OnDiscussionTextNodeEnd();
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

        #region External Events
        public void OnDiscussionWindowAwake(DiscussionTextOnlyNode discussionNode, Transform position)
        {
            OpenedDiscussion = Instantiate(PrefabContainer.Instance.DiscussionUIPrefab, GameCanvas.transform, false);
            OpenedDiscussion.transform.localScale = Vector3.zero;
            OpenedDiscussion.InitializeDependencies();
            OpenedDiscussion.OnDiscussionWindowAwake(discussionNode, position, ref this.AdventureStaticConfiguration.DiscussionTestRepertoire);
        }

        public IEnumerator PlayDiscussionCloseAnimation()
        {
            return OpenedDiscussion.PlayDiscussionCloseAnimation();
        }

        public void OnChoicePopupAwake(List<DiscussionChoice> nexDiscussionChoices)
        {
            OpenedChoicePopup = Instantiate(PrefabContainer.Instance.ChoicePopupPrefab, OpenedDiscussion.transform);
            OpenedChoicePopup.OnChoicePopupAwake(nexDiscussionChoices, Vector2.zero, ref this.AdventureStaticConfiguration.DiscussionTestRepertoire);
        }

        public void OnDiscussionWindowSleep()
        {
            OpenedDiscussion.OnDiscussionWindowSleep();

            if (OpenedChoicePopup != null)
            {
                OpenedChoicePopup = null;
            }
            Destroy(OpenedDiscussion.gameObject);
            OpenedDiscussion = null;
        }
        #endregion

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