using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{

    public class DiscussionWindow : MonoBehaviour, DiscussionWindowOnTextFinishedWritingListener, DiscussionHeightChangeListener
    {
        public Material TestMaterial;
        public Texture2D DebugTexture;

        #region Constants
        private const string TEXT_AREA_OBJECT_NAME = "TextArea";
        private const string DISCUSSION_WINDOW_OBJECT_NAME = "DiscussionWindow";
        private const string WORKFLOW_ICON_IMAGE_CONTAINER_NAME = "WorkflowIconImageContainer";
        #endregion
        
        public DiscussionWriterComponent DiscussionWriterComponent;
        public DiscussionWindowDimensionsTransitionComponent DiscussionWindowDimensionsTransitionComponent;
        public GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent;
        
        #region Instance
        public static DiscussionWindow Instanciate(Canvas canvas)
        {
            return MonoBehaviour.Instantiate(CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CorePrefabConfiguration.DiscussionUIPrefab, canvas.transform, false);
        }
        #endregion

        #region Internal Dependencies
        private DiscussionWriterManager DiscussionWriterManager;
        private DiscussionWorkflowManager DiscussionWorkflowManager;

        private DiscussionWindowPositioner DiscussionWindowPositioner;
        private DiscussionWindowDimensionsTransitionManager DiscussionWindowDimensionsTransitionManager;
        private DiscussionWindowAnimationManager DiscussionWindowAnimationManager;

        private GeneratedText currentDiscussionText;
        private Text textAreaText;
        #endregion

        private DiscussionTextInherentData currentWindowDiscussionTextInherentData;

        private Action OnExitAnimationFinished;

        public void InitializeDependencies(Action OnExitAnimationFinished, bool displayWorkflowIcon = true)
        {
            var textAreaObject = gameObject.FindChildObjectRecursively(TEXT_AREA_OBJECT_NAME);
            this.textAreaText = textAreaObject.GetComponent<Text>();

            var discussionWindowObject = gameObject.FindChildObjectRecursively(DISCUSSION_WINDOW_OBJECT_NAME);

            var discussionAnimator = GetComponent<Animator>();
            var discussionWindowObjectTransform = (RectTransform)discussionWindowObject.transform;

            this.OnExitAnimationFinished = OnExitAnimationFinished;

            ((RectTransform)(textAreaObject.transform)).offsetMin = new Vector2(GeneratedTextDimensionsComponent.MarginLeft, GeneratedTextDimensionsComponent.MarginDown);
            ((RectTransform)(textAreaObject.transform)).offsetMax = new Vector2(-GeneratedTextDimensionsComponent.MarginRight, -GeneratedTextDimensionsComponent.MarginUp);

            DiscussionWindowPositioner = new DiscussionWindowPositioner(Camera.main, transform);
            DiscussionWindowDimensionsTransitionManager = new DiscussionWindowDimensionsTransitionManager(DiscussionWindowDimensionsTransitionComponent, discussionWindowObjectTransform);
            DiscussionWindowAnimationManager = new DiscussionWindowAnimationManager(discussionAnimator);

            DiscussionWriterManager = new DiscussionWriterManager(this, DiscussionWriterComponent, this.textAreaText);
            DiscussionWorkflowManager = new DiscussionWorkflowManager(CoreGameSingletonInstances.CoreConfigurationManager.InputConfiguration(), (RectTransform)gameObject.FindChildObjectRecursively(WORKFLOW_ICON_IMAGE_CONTAINER_NAME).transform, displayWorkflowIcon);
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

        private void OnDrawGizmos()
        {
            if (this.textAreaText != null && this.DebugTexture != null)
            {
                this.textAreaText.cachedTextGenerator.GetCharactersArray().ToList().ForEach((c) =>
                {
                    var p3 = ((RectTransform)this.textAreaText.transform).position;
                    Gizmos.DrawGUITexture(new Rect(new Vector2(p3.x + c.cursorPos.x, p3.y + c.cursorPos.y), new Vector2(3, 3)), this.DebugTexture);
                });
            }
        }

        #region External Events
        public void OnDiscussionWindowAwakeV2(DiscussionTextInherentData discussionText, Vector3 worldPosition, WindowPositionType WindowPositionType)
        {
            this.currentWindowDiscussionTextInherentData = discussionText;
            DiscussionWindowAnimationManager.PlayEnterAnimation();
            DiscussionWindowPositioner.SetTransformToFollow(worldPosition, WindowPositionType);
            InitializeDiscussionWindow(discussionText.Text, discussionText.InputParameters.AsReadOnly());
        }

        public void OnDiscussionWindowSleep()
        {
            this.currentDiscussionText.OnDiscussionTerminated();
            DiscussionWorkflowManager.OnDiscussionWindowSleep();
        }

        private void InitializeDiscussionWindow(string text, ReadOnlyCollection<InputParameter> InputParameters)
        {
            DiscussionWorkflowManager.OnDiscussionWindowAwake();
            this.currentDiscussionText = new GeneratedText(text, new GeneratedTextParameter(InputParameters, CoreGameSingletonInstances.CoreConfigurationManager.InputConfiguration()), GeneratedTextDimensionsComponent,
                this, this.textAreaText);
            this.OnDiscussionStartWriting();
        }

        public void ProcessDiscussionContinue()
        {
            this.currentDiscussionText.OnDiscussionContinue();
            this.OnDiscussionStartWriting();
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

        private void OnDiscussionStartWriting()
        {
            this.currentDiscussionText.ComputeTruncatedText();
            this.OnHeightChange(this.currentDiscussionText.GetWindowHeight(this.currentDiscussionText.GetDisplayedLineNb()));
            this.OnWidthChange(this.GeneratedTextDimensionsComponent.MaxWindowWidth);
            DiscussionWriterManager.OnDiscussionTextStartWriting();
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

        public void Tick(float d, GeneratedText discussionText)
        {
            if (isTextWriting)
            {
                timeElapsed += d;

                if (discussionText.IsAllowedToIncrementEngine())
                {
                    while (timeElapsed >= DiscussionWriterComponent.LetterDisplayIntervalTime)
                    {
                        timeElapsed -= DiscussionWriterComponent.LetterDisplayIntervalTime;
                        discussionText.Increment();
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

        private InputImageType InputImageType;

        public DiscussionWorkflowManager(InputConfiguration InputConfiguration, RectTransform imageIconContainer, bool displayWorkflowIcon)
        {
            if (displayWorkflowIcon)
            {
                this.InputImageType = InputImageType.Instantiate(InputConfiguration.ConfigurationInherentData[GameConfigurationID.InputID.ACTION_DOWN], imageIconContainer, true);
                this.InputImageType.transform.localPosition = Vector3.zero;
                this.InputImageType.gameObject.SetActive(false);
            }
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
                    isWaitningForEnd = false;
                }
                else
                {
                    isWaitingForContinue = false;
                    isWaitningForEnd = true;
                }

                if (this.InputImageType != null)
                {
                    this.InputImageType.gameObject.SetActive(true);
                }

            }
        }

        public void OnDiscussionWindowAwake()
        {
            ResetState();
        }

        private void ResetState()
        {
            if (this.InputImageType != null)
            {
                this.InputImageType.gameObject.SetActive(false);
            }
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

        private Vector3 worldPositionToFollow;
        private WindowPositionType WindowPositionType;

        public DiscussionWindowPositioner(Camera camera, Transform discussionTransform)
        {
            this.camera = camera;
            this.discussionTransform = discussionTransform;
        }

        public void SetTransformToFollow(Vector3 worldPosition, WindowPositionType WindowPositionType)
        {
            this.worldPositionToFollow = worldPosition;
            this.WindowPositionType = WindowPositionType;
        }

        public void Tick()
        {
            if (this.WindowPositionType == WindowPositionType.WORLD)
            {
                this.discussionTransform.position = camera.WorldToScreenPoint(worldPositionToFollow);
            }
            else if (this.WindowPositionType == WindowPositionType.SCREEN)
            {
                this.discussionTransform.position = worldPositionToFollow;
            }
        }
    }

    public enum WindowPositionType
    {
        WORLD = 0,
        SCREEN = 1
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