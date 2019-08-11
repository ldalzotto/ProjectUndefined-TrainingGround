using System;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{

    public class DiscussionWindow : MonoBehaviour, DiscussionWindowOnTextFinishedWritingListener, DiscussionHeightChangeListener
    {
        private const string TEXT_AREA_OBJECT_NAME = "TextArea";
        private const string DISCUSSION_WINDOW_OBJECT_NAME = "DiscussionWindow";
        private const string CONTINUE_ICON_OBJECT_NAME = "ContinueIcon";
        private const string END_ICON_OBJECT_NAME = "EndIcon";

        public TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent;
        public DiscussionWriterComponent DiscussionWriterComponent;
        public DiscussionWindowDimensionsTransitionComponent DiscussionWindowDimensionsTransitionComponent;
        public DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;

        #region Internal Dependencies
        private DiscussionWriterManager DiscussionWriterManager;
        private DiscussionWorkflowManager DiscussionWorkflowManager;

        private DiscussionWindowPositioner DiscussionWindowPositioner;
        private DiscussionWindowDimensionsTransitionManager DiscussionWindowDimensionsTransitionManager;
        private DiscussionWindowAnimationManager DiscussionWindowAnimationManager;

        private DiscussionText currentDiscussionText;
        private RectTransform discussionWindowObjectTransform;
        private Text textAreaText;
        #endregion
        private Action OnExitAnimationFinished;

        public void InitializeDependencies(Action OnExitAnimationFinished)
        {
            var textAreaObject = gameObject.FindChildObjectRecursively(TEXT_AREA_OBJECT_NAME);
            this.textAreaText = textAreaObject.GetComponent<Text>();

            var discussionWindowObject = gameObject.FindChildObjectRecursively(DISCUSSION_WINDOW_OBJECT_NAME);

            var discussionAnimator = GetComponent<Animator>();
            this.discussionWindowObjectTransform = (RectTransform)discussionWindowObject.transform;

            this.OnExitAnimationFinished = OnExitAnimationFinished;

            ((RectTransform)(textAreaObject.transform)).offsetMin = new Vector2(DiscussionWindowDimensionsComponent.MarginLeft, DiscussionWindowDimensionsComponent.MarginDown);
            ((RectTransform)(textAreaObject.transform)).offsetMax = new Vector2(-DiscussionWindowDimensionsComponent.MarginRight, -DiscussionWindowDimensionsComponent.MarginUp);

            DiscussionWindowPositioner = new DiscussionWindowPositioner(Camera.main, transform);
            DiscussionWindowDimensionsTransitionManager = new DiscussionWindowDimensionsTransitionManager(DiscussionWindowDimensionsTransitionComponent, this.discussionWindowObjectTransform);
            DiscussionWindowAnimationManager = new DiscussionWindowAnimationManager(discussionAnimator);
            
            DiscussionWriterManager = new DiscussionWriterManager(this, DiscussionWriterComponent, this.textAreaText);
            DiscussionWorkflowManager = new DiscussionWorkflowManager(gameObject.FindChildObjectRecursively(CONTINUE_ICON_OBJECT_NAME), gameObject.FindChildObjectRecursively(END_ICON_OBJECT_NAME));
        }

        public void Tick(float d)
        {
            //We write first
            DiscussionWriterManager.Tick(d, this.currentDiscussionText);

            DiscussionWindowPositioner.Tick();

            //Window dimension transitions if detected
            DiscussionWindowDimensionsTransitionManager.Tick(d);

            if (DiscussionWindowAnimationManager.Tick())
            {
                this.OnExitAnimationFinished.Invoke();
            }
        }

        #region External Events
        public void OnDiscussionWindowAwake(string fullText, Transform position)
        {
            DiscussionWindowAnimationManager.PlayEnterAnimation();
            DiscussionWindowPositioner.SetTransformToFollow(position);
            InitializeDiscussionWindow(fullText);
        }

        public void OnDiscussionWindowSleep()
        {
            DiscussionWorkflowManager.OnDiscussionWindowSleep();
        }

        private void InitializeDiscussionWindow(string fullTextContent)
        {
            DiscussionWorkflowManager.OnDiscussionWindowAwake();

            this.currentDiscussionText = new DiscussionText(fullTextContent, this.DiscussionWindowDimensionsComponent, this.TextOnlyDiscussionWindowDimensionsComponent, this);
            this.currentDiscussionText.ComputeTruncatedText(this.discussionWindowObjectTransform, this.textAreaText);
            this.OnHeightChange(this.currentDiscussionText.GetWindowHeight(this.currentDiscussionText.GetDisplayedLineNb(), this.textAreaText));
            this.OnWidthChange(this.currentDiscussionText.GetFinalWidth());

            DiscussionWriterManager.OnDiscussionTextStartWriting();
        }

        public void ProcessDiscussionContinue()
        {
            InitializeDiscussionWindow(this.currentDiscussionText.OverlappedText);
        }

        public void PlayDiscussionCloseAnimation()
        {
            DiscussionWindowAnimationManager.PlayExitAnimation();
        }

        public void OnHeightChange(float newHeight)
        {
            DiscussionWindowDimensionsTransitionManager.OnHeightChange(newHeight);
        }

        public void OnWidthChange(float newWidth)
        {
            DiscussionWindowDimensionsTransitionManager.OnWidthChange(newWidth);
        }
        #endregion

        #region Internal Events
        public void OnTextFinishedWriting()
        {
            DiscussionWorkflowManager.OnTextFinishedWriting(this.currentDiscussionText.OverlappedText);
        }

        #endregion

        #region Functional Counditions
        public bool IsWaitingForCloseInput()
        {
            return DiscussionWorkflowManager.IsWaitningForEnd;
        }
        public bool IsWaitingForContinueInput()
        {
            return DiscussionWorkflowManager.IsWaitingForContinue;
        }
        public bool IsWriting()
        {
            return DiscussionWriterManager.IsTextWriting;
        }
        public bool IsExitAnimationPlaying()
        {
            return DiscussionWindowAnimationManager.IsAnimationExitingPlaying;
        }
        #endregion
    }

    #region Discussion Window Dimensions
    [System.Serializable]
    public class TextOnlyDiscussionWindowDimensionsComponent
    {
        public int MaxLineDisplayed;
    }
    #endregion

    #region Discussion Window Text Writer
    [System.Serializable]
    public class DiscussionWriterComponent
    {
        private float letterDisplayIntervalTime = 0.01f;

        public float LetterDisplayIntervalTime { get => letterDisplayIntervalTime; }
    }

    public interface DiscussionWindowOnTextFinishedWritingListener
    {
        void OnTextFinishedWriting();
    }

    public class DiscussionWriterManager
    {
        private DiscussionWriterComponent DiscussionWriterComponent;
        private Text textAreaText;

        private float timeElapsed;
        private bool isTextWriting;

        private DiscussionWindowOnTextFinishedWritingListener DiscussionWindowOnTextFinishedWritingListener;

        public DiscussionWriterManager(DiscussionWindowOnTextFinishedWritingListener DiscussionWindowOnTextFinishedWritingListener, DiscussionWriterComponent discussionWriterComponent,
            Text textAreaText)
        {
            this.DiscussionWindowOnTextFinishedWritingListener = DiscussionWindowOnTextFinishedWritingListener;
            DiscussionWriterComponent = discussionWriterComponent;
            this.textAreaText = textAreaText;
        }

        public bool IsTextWriting { get => isTextWriting; }

        public void Tick(float d, DiscussionText discussionText)
        {
            if (isTextWriting)
            {
                textAreaText.text = discussionText.GetCurrentDisplayedText();
                timeElapsed += d;

                if (discussionText.IsAllowedToIncrementEngine())
                {
                    while (timeElapsed >= DiscussionWriterComponent.LetterDisplayIntervalTime)
                    {
                        timeElapsed -= DiscussionWriterComponent.LetterDisplayIntervalTime;
                        discussionText.Increment(textAreaText);
                    }
                }
                else
                {
                    isTextWriting = false;
                    this.DiscussionWindowOnTextFinishedWritingListener.OnTextFinishedWriting();
                }
            }

        }

        public void OnDiscussionTextStartWriting()
        {
            isTextWriting = true;
            timeElapsed = 0f;
        }
    }

    #endregion

    #region Discussion Window Workflow
    class DiscussionWorkflowManager
    {
        private GameObject ContinueIcon;
        private GameObject EndIcon;

        public DiscussionWorkflowManager(GameObject continueIcon, GameObject endIcon)
        {
            ContinueIcon = continueIcon;
            EndIcon = endIcon;
            OnDiscussionWindowAwake();
        }

        private bool isWaitingForContinue;
        private bool isWaitningForEnd;

        public bool IsWaitingForContinue { get => isWaitingForContinue; }
        public bool IsWaitningForEnd { get => isWaitningForEnd; }

        public void OnTextFinishedWriting(string overlappedOnlyText)
        {
            if (overlappedOnlyText != null)
            {
                if (overlappedOnlyText != "")
                {
                    isWaitingForContinue = true;
                    ContinueIcon.SetActive(true);
                    isWaitningForEnd = false;
                    EndIcon.SetActive(false);
                }
                else
                {
                    isWaitingForContinue = false;
                    ContinueIcon.SetActive(false);
                    isWaitningForEnd = true;
                    EndIcon.SetActive(true);
                }
            }
        }

        public void OnDiscussionWindowAwake()
        {
            ResetState();
        }

        private void ResetState()
        {
            ContinueIcon.SetActive(false);
            EndIcon.SetActive(false);
            isWaitingForContinue = false;
            isWaitningForEnd = false;
        }

        public void OnDiscussionWindowSleep()
        {
            ResetState();
        }
    }
    #endregion


    #region Discussion Window Position
    class DiscussionWindowPositioner
    {
        private Camera camera;
        private Transform discussionTransform;
        private Transform worldTransformToFollow;

        public DiscussionWindowPositioner(Camera camera, Transform discussionTransform)
        {
            this.camera = camera;
            this.discussionTransform = discussionTransform;
        }

        public void SetTransformToFollow(Transform worldTransformToFollow)
        {
            this.worldTransformToFollow = worldTransformToFollow;
        }

        public void Tick()
        {
            if (worldTransformToFollow != null)
            {
                if (worldTransformToFollow.GetType() == typeof(RectTransform))
                {
                    this.discussionTransform.position = worldTransformToFollow.position;
                }
                else
                {
                    this.discussionTransform.position = camera.WorldToScreenPoint(worldTransformToFollow.position);
                }
            }
        }
    }
    #endregion

    #region DiscussionWindow Dimensions
    [System.Serializable]
    public class DiscussionWindowDimensionsComponent
    {
        public float MarginLeft;
        public float MarginRight;
        public float MarginUp;
        public float MarginDown;
        public float MaxWindowWidth;
        public float MinWindowHeight;
    }
    #endregion

    #region Dimensions Transitions
    [System.Serializable]
    public class DiscussionWindowDimensionsTransitionComponent
    {
        public float HeightTransitionsSpeed;
    }

    class DiscussionWindowDimensionsTransitionManager
    {

        private DiscussionWindowDimensionsTransitionComponent DiscussionWindowDimensionsTransitionComponent;
        private RectTransform discussionWindowTransform;

        public DiscussionWindowDimensionsTransitionManager(DiscussionWindowDimensionsTransitionComponent discussionWindowDimensionsTransitionComponent, RectTransform discussionWindowTransform)
        {
            DiscussionWindowDimensionsTransitionComponent = discussionWindowDimensionsTransitionComponent;
            this.discussionWindowTransform = discussionWindowTransform;
        }

        private float targetWindowheight;
        private bool heightUpdating;

        public void Tick(float d)
        {
            if (heightUpdating)
            {
                discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, Mathf.Lerp(discussionWindowTransform.sizeDelta.y, targetWindowheight, d * DiscussionWindowDimensionsTransitionComponent.HeightTransitionsSpeed));
                if (Mathf.Abs(targetWindowheight - discussionWindowTransform.sizeDelta.y) <= 0.05)
                {
                    discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, targetWindowheight);
                    heightUpdating = false;
                }
            }
        }

        public void OnHeightChange(float newHeight)
        {
            heightUpdating = true;
            targetWindowheight = newHeight;
        }

        public void OnWidthChange(float newWidth)
        {
            discussionWindowTransform.sizeDelta = new Vector2(newWidth, discussionWindowTransform.sizeDelta.y);
        }
    }
    #endregion

    #region Discussion Window Animation
    class DiscussionWindowAnimationManager
    {
        private const string ENTER_ANIMATION_NAME = "DiscussionWindowEnterAnimation";
        private const string EXIT_ANIMATION_NAME = "DiscussionWindowExitAnimation";

        private Animator DiscussionAnimator;

        private bool isAnimationExitingPlaying;

        public DiscussionWindowAnimationManager(Animator discussionAnimator)
        {
            DiscussionAnimator = discussionAnimator;
        }

        public bool IsAnimationExitingPlaying { get => isAnimationExitingPlaying; }

        public void PlayEnterAnimation()
        {
            DiscussionAnimator.Play(ENTER_ANIMATION_NAME);
        }

        public bool Tick()
        {
            if (this.isAnimationExitingPlaying)
            {
                isAnimationExitingPlaying = WaitForEndOfAnimation.IsAnimationPlaying(DiscussionAnimator, EXIT_ANIMATION_NAME, 0, false);
                if (!isAnimationExitingPlaying)
                {
                    return true;
                }
            }
            return false;
        }

        public void PlayExitAnimation()
        {
            DiscussionAnimator.Play(EXIT_ANIMATION_NAME);
            DiscussionAnimator.Update(0.01f);
            isAnimationExitingPlaying = true;
        }
    }
    #endregion
}