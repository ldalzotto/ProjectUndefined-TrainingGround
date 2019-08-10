using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{

    public class DiscussionWindow : MonoBehaviour
    {

        private const string TEXT_AREA_OBJECT_NAME = "TextArea";
        private const string DISCUSSION_WINDOW_OBJECT_NAME = "DiscussionWindow";
        private const string CONTINUE_ICON_OBJECT_NAME = "ContinueIcon";
        private const string END_ICON_OBJECT_NAME = "EndIcon";

        public TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent;
        public DiscussionWriterComponent DiscussionWriterComponent;
        public DiscussionWindowDimensionsTransitionComponent DiscussionWindowDimensionsTransitionComponent;
        public DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;

        private DiscussionWindowDimensionsManager DiscussionWindowDimensionsManager;
        private DiscussionWindowDimensionsComputation DiscussionWindowDimensionsComputation;
        private DiscussionWriterManager DiscussionWriterManager;
        private DiscussionWorkflowManager DiscussionWorkflowManager;

        private DiscussionWindowPositioner DiscussionWindowPositioner;
        private DiscussionWindowDimensionsTransitionManager DiscussionWindowDimensionsTransitionManager;
        private DiscussionWindowAnimationManager DiscussionWindowAnimationManager;

        private Action OnExitAnimationFinished;

        public void InitializeDependencies(Action OnExitAnimationFinished)
        {
            var textAreaObject = gameObject.FindChildObjectRecursively(TEXT_AREA_OBJECT_NAME);
            var discussionWindowObject = gameObject.FindChildObjectRecursively(DISCUSSION_WINDOW_OBJECT_NAME);

            var discussionAnimator = GetComponent<Animator>();

            this.OnExitAnimationFinished = OnExitAnimationFinished;
            DiscussionWindowDimensionsComputation = new DiscussionWindowDimensionsComputation(DiscussionWindowDimensionsComponent, textAreaObject, (RectTransform)discussionWindowObject.transform, this.TextOnlyDiscussionWindowDimensionsComponent);
            DiscussionWindowPositioner = new DiscussionWindowPositioner(Camera.main, transform);
            DiscussionWindowDimensionsTransitionManager = new DiscussionWindowDimensionsTransitionManager(DiscussionWindowDimensionsTransitionComponent, (RectTransform)discussionWindowObject.transform);
            DiscussionWindowAnimationManager = new DiscussionWindowAnimationManager(discussionAnimator);

            DiscussionWindowDimensionsManager = new DiscussionWindowDimensionsManager(this, textAreaObject, discussionWindowObject, TextOnlyDiscussionWindowDimensionsComponent, DiscussionWindowDimensionsComputation);
            DiscussionWriterManager = new DiscussionWriterManager(() => { this.OnTextFinishedWriting(); }, DiscussionWriterComponent, textAreaObject.GetComponent<Text>(), (RectTransform)discussionWindowObject.transform, DiscussionWindowDimensionsComputation);
            DiscussionWorkflowManager = new DiscussionWorkflowManager(gameObject.FindChildObjectRecursively(CONTINUE_ICON_OBJECT_NAME), gameObject.FindChildObjectRecursively(END_ICON_OBJECT_NAME));
        }

        public void Tick(float d)
        {
            //We write first
            DiscussionWriterManager.Tick(d);

            DiscussionWindowPositioner.Tick();
            DiscussionWindowDimensionsManager.Tick(d);

            //Window dimension transitions if detected
            DiscussionWindowDimensionsTransitionManager.Tick(d);

            if (DiscussionWindowAnimationManager.Tick())
            {
                this.OnExitAnimationFinished.Invoke();
            }

        }

        public void OnGUIDraw()
        {
            DiscussionWindowDimensionsManager.OnGUIDraw();
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
            fullTextContent = Regex.Unescape(fullTextContent);
            DiscussionWindowDimensionsManager.InitializeDiscussionWindow(fullTextContent);
            var truncatedText = DiscussionWindowDimensionsManager.ComputeTrucatedText(fullTextContent);
            DiscussionWriterManager.OnDiscussionTextStartWriting(truncatedText);
        }

        public void ProcessDiscussionContinue()
        {
            InitializeDiscussionWindow(DiscussionWindowDimensionsManager.OverlappedOnlyText);
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
            DiscussionWorkflowManager.OnTextFinishedWriting(DiscussionWindowDimensionsManager.OverlappedOnlyText);
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

    class DiscussionWindowDimensionsManager
    {
        private DiscussionWindow DiscussionBaseReference;
        private TextOnlyDiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;
        private Text textAreaText;
        private RectTransform discussionWindowTransform;

        private IDiscussionWindowDimensionsComputation discussionWindowDimensionsComputation;
        private int displayedLineNB;
        private int nonOverlappedLineNb;
        private TextGenerator cachedTextGenerator;

        private string overlappedOnlyText;

        public string OverlappedOnlyText { get => overlappedOnlyText; }

        public DiscussionWindowDimensionsManager(DiscussionWindow DiscussionBaseReference, GameObject textAreaObject,
                        GameObject discussionWindowObject, TextOnlyDiscussionWindowDimensionsComponent textOnlyDiscussionWindowDimensionsComponent,
                        IDiscussionWindowDimensionsComputation discussionWindowDimensionsComputation)
        {
            this.displayedLineNB = 1;
            this.DiscussionBaseReference = DiscussionBaseReference;
            this.textAreaText = textAreaObject.GetComponent<Text>();
            this.discussionWindowTransform = (RectTransform)discussionWindowObject.transform;
            this.DiscussionWindowDimensionsComponent = textOnlyDiscussionWindowDimensionsComponent;
            this.discussionWindowDimensionsComputation = discussionWindowDimensionsComputation;
        }

        public void OnGUIDraw()
        {
            cachedTextGenerator = textAreaText.cachedTextGenerator;
        }

        private void CalculateNonOverlappedLineNb()
        {
            this.nonOverlappedLineNb = Mathf.FloorToInt((textAreaText.preferredHeight) / discussionWindowDimensionsComputation.GetSingleLineHeightWithLineSpace());
        }

        public void Tick(float d)
        {
            if (IsCurrentTextOverlaps())
            {
                this.displayedLineNB = Mathf.Min(this.displayedLineNB + 1, DiscussionWindowDimensionsComponent.MaxLineDisplayed);
                DiscussionBaseReference.OnHeightChange(discussionWindowDimensionsComputation.GetWindowHeight(this.displayedLineNB));
            }
        }

        private bool IsCurrentTextOverlaps()
        {
            return (discussionWindowDimensionsComputation.GetCurrentWindowHeight() > discussionWindowDimensionsComputation.GetWindowHeight(this.displayedLineNB));
        }

        public void InitializeDiscussionWindow(string fullTextContent)
        {
            this.displayedLineNB = 1;

            this.ForceUpdateTextCachedTextGenerator(fullTextContent);
            DiscussionBaseReference.OnHeightChange(discussionWindowDimensionsComputation.GetWindowHeight(this.displayedLineNB));
            DiscussionBaseReference.OnWidthChange(discussionWindowDimensionsComputation.GetPreferredWindowWidthClamped());

            CalculateNonOverlappedLineNb();
            this.ForceUpdateTextCachedTextGenerator("");
        }

        public string ComputeTrucatedText(string fullTextContent)
        {
            var initialSizeDelta = discussionWindowTransform.sizeDelta;

            discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, discussionWindowDimensionsComputation.GetMaxWindowHeight());
            this.ForceUpdateTextCachedTextGenerator(fullTextContent);

            var truncatedText = textAreaText.text.Substring(0, textAreaText.cachedTextGenerator.characterCountVisible);
            overlappedOnlyText = textAreaText.text.Substring(textAreaText.cachedTextGenerator.characterCountVisible, textAreaText.text.Length - textAreaText.cachedTextGenerator.characterCountVisible);

            discussionWindowTransform.sizeDelta = initialSizeDelta;
            this.ForceUpdateTextCachedTextGenerator("");

            return truncatedText;
        }

        private void ForceUpdateTextCachedTextGenerator(string textContent)
        {
            textAreaText.text = textContent;
            Canvas.ForceUpdateCanvases();
        }
    }

    #endregion

    #region Discussion Window Text Writer
    [System.Serializable]
    public class DiscussionWriterComponent
    {
        private float letterDisplayIntervalTime = 0.01f;

        public float LetterDisplayIntervalTime { get => letterDisplayIntervalTime; }
    }

    public class DiscussionWriterManager
    {
        private char[] TrimmedCharForSanitaze = new char[] { ' ', '\n' };

        private DiscussionWriterComponent DiscussionWriterComponent;
        private Text textAreaText;
        private RectTransform discussionWindowTransform;
        private IDiscussionWindowDimensionsComputation discussionWindowDimensionsComputation;

        private string currentDisplayedText;
        private string truncatedTargetText;
        private float timeElapsed;
        private bool isTextWriting;

        public delegate void TextFinishedWritingHanlder();
        public event TextFinishedWritingHanlder OnTextFinishedWriting;

        public DiscussionWriterManager(TextFinishedWritingHanlder TextFinishedWritingHanlder, DiscussionWriterComponent discussionWriterComponent, Text textAreaText, RectTransform discussionWindowTransform, IDiscussionWindowDimensionsComputation discussionWindowDimensionsComputation)
        {
            this.discussionWindowTransform = discussionWindowTransform;
            this.discussionWindowDimensionsComputation = discussionWindowDimensionsComputation;
            OnTextFinishedWriting += TextFinishedWritingHanlder;
            DiscussionWriterComponent = discussionWriterComponent;
            this.textAreaText = textAreaText;
        }

        public bool IsTextWriting { get => isTextWriting; }

        public void Tick(float d)
        {
            if (isTextWriting)
            {
                textAreaText.text = currentDisplayedText;
                timeElapsed += d;

                if (currentDisplayedText.Length != truncatedTargetText.Length)
                {
                    while (timeElapsed >= DiscussionWriterComponent.LetterDisplayIntervalTime)
                    {
                        timeElapsed -= DiscussionWriterComponent.LetterDisplayIntervalTime;
                        if (currentDisplayedText.Length < truncatedTargetText.Length)
                        {
                            currentDisplayedText += truncatedTargetText[currentDisplayedText.Length];
                        }
                    }
                }
                else
                {
                    isTextWriting = false;
                    if (OnTextFinishedWriting != null)
                    {
                        OnTextFinishedWriting.Invoke();
                    }
                }
            }

        }

        public void OnDiscussionTextStartWriting(string truncatedTargetText)
        {
            var sanitizedTruncatedTargetText = truncatedTargetText.Trim(this.TrimmedCharForSanitaze);

            var initialDiscussionWindowAreaSize = this.discussionWindowTransform.sizeDelta;
            this.discussionWindowTransform.sizeDelta = new Vector2(this.discussionWindowTransform.sizeDelta.x, this.discussionWindowDimensionsComputation.GetMaxWindowHeight());

            this.textAreaText.text = sanitizedTruncatedTargetText;
            Canvas.ForceUpdateCanvases();
            List<string> finalRenderedLines = new List<string>();
            for (int i = 0; i < this.textAreaText.cachedTextGenerator.lines.Count; i++)
            {
                int startIndex = this.textAreaText.cachedTextGenerator.lines[i].startCharIdx;
                int endIndex = (i == this.textAreaText.cachedTextGenerator.lines.Count - 1) ? this.textAreaText.text.Length
                    : this.textAreaText.cachedTextGenerator.lines[i + 1].startCharIdx;
                int length = endIndex - startIndex;
                finalRenderedLines.Add(this.textAreaText.text.Substring(startIndex, length).Trim(this.TrimmedCharForSanitaze));
            }



            this.textAreaText.text = "";
            this.discussionWindowTransform.sizeDelta = initialDiscussionWindowAreaSize;
            Canvas.ForceUpdateCanvases();

            this.truncatedTargetText = String.Join("\n", finalRenderedLines.ToArray());

            isTextWriting = true;
            timeElapsed = 0f;
            currentDisplayedText = "";
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

    public interface IDiscussionWindowDimensionsComputation
    {
        float GetPreferredWindowWidthClamped();
        float GetCurrentWindowHeight();
        float GetSingleLineHeightWithLineSpace();
        float GetWindowHeight(int lineNB);
        float GetMaxWindowHeight();
    }

    class DiscussionWindowDimensionsComputation : IDiscussionWindowDimensionsComputation
    {
        private DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;
        private TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent;

        private Text textAreaText;
        private RectTransform discussionWindowTransform;

        public DiscussionWindowDimensionsComputation(DiscussionWindowDimensionsComponent discussionWindowDimensionsComponent, GameObject textAreaObject,
                RectTransform discussionWindowTransform, TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent)
        {
            DiscussionWindowDimensionsComponent = discussionWindowDimensionsComponent;
            this.TextOnlyDiscussionWindowDimensionsComponent = TextOnlyDiscussionWindowDimensionsComponent;

            var textAreaTransform = (RectTransform)textAreaObject.transform;
            this.textAreaText = textAreaObject.GetComponent<Text>();
            this.discussionWindowTransform = discussionWindowTransform;
            textAreaTransform.offsetMin = new Vector2(discussionWindowDimensionsComponent.MarginLeft, discussionWindowDimensionsComponent.MarginDown);
            textAreaTransform.offsetMax = new Vector2(-discussionWindowDimensionsComponent.MarginRight, -discussionWindowDimensionsComponent.MarginUp);
        }

        public float GetCurrentWindowHeight()
        {
            return Mathf.Max(DiscussionWindowDimensionsComponent.MinWindowHeight, textAreaText.preferredHeight + (DiscussionWindowDimensionsComponent.MarginDown + DiscussionWindowDimensionsComponent.MarginUp));
        }

        public float GetPreferredWindowWidthClamped()
        {
            return Mathf.Min(textAreaText.preferredWidth + (DiscussionWindowDimensionsComponent.MarginLeft + DiscussionWindowDimensionsComponent.MarginRight), DiscussionWindowDimensionsComponent.MaxWindowWidth);
        }

        public float GetSingleLineHeightWithLineSpace()
        {
            return (textAreaText.font.lineHeight + textAreaText.lineSpacing);
        }

        public float GetMaxWindowHeight()
        {
            return this.GetWindowHeight(this.TextOnlyDiscussionWindowDimensionsComponent.MaxLineDisplayed);
        }

        public float GetWindowHeight(int lineNb)
        {
            return Mathf.Max(DiscussionWindowDimensionsComponent.MinWindowHeight, ((textAreaText.font.lineHeight + textAreaText.lineSpacing) * lineNb) + (DiscussionWindowDimensionsComponent.MarginDown + DiscussionWindowDimensionsComponent.MarginUp));
        }
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