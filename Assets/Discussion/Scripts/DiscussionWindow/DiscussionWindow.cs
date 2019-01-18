using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    private TextOnlyDiscussionWindowDimensionsManager TextDiscussionWindowDimensionsManager;
    private DiscussionWindowDimensionsManager DiscussionWindowDimensionsManager;
    private DiscussionWriterManager DiscussionWriterManager;
    private DiscussionWorkflowManager DiscussionWorkflowManager;

    private DiscussionWindowPositioner DiscussionWindowPositioner;
    private DiscussionWindowDimensionsTransitionManager DiscussionWindowDimensionsTransitionManager;
    private DiscussionWindowAnimationManager DiscussionWindowAnimationManager;

    private DiscussionEventHandler DiscussionEventHandler;

    public void InitializeDependencies()
    {
        #region External Event hanlder
        DiscussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
        #endregion

        var textAreaObject = gameObject.FindChildObjectRecursively(TEXT_AREA_OBJECT_NAME);
        var discussionWindowObject = gameObject.FindChildObjectRecursively(DISCUSSION_WINDOW_OBJECT_NAME);

        var discussionAnimator = GetComponent<Animator>();

        DiscussionWindowDimensionsManager = new DiscussionWindowDimensionsManager(DiscussionWindowDimensionsComponent, textAreaObject, (RectTransform)discussionWindowObject.transform);
        DiscussionWindowPositioner = new DiscussionWindowPositioner(Camera.main, transform);
        DiscussionWindowDimensionsTransitionManager = new DiscussionWindowDimensionsTransitionManager(DiscussionWindowDimensionsTransitionComponent, (RectTransform)discussionWindowObject.transform);
        DiscussionWindowAnimationManager = new DiscussionWindowAnimationManager(discussionAnimator, DiscussionEventHandler);

        TextDiscussionWindowDimensionsManager = new TextOnlyDiscussionWindowDimensionsManager(this, textAreaObject, discussionWindowObject, TextOnlyDiscussionWindowDimensionsComponent, DiscussionWindowDimensionsManager);
        DiscussionWriterManager = new DiscussionWriterManager(this, DiscussionWriterComponent, textAreaObject.GetComponent<Text>());
        DiscussionWorkflowManager = new DiscussionWorkflowManager(gameObject.FindChildObjectRecursively(CONTINUE_ICON_OBJECT_NAME), gameObject.FindChildObjectRecursively(END_ICON_OBJECT_NAME));
    }

    public void Tick(float d)
    {
        DiscussionWindowDimensionsTransitionManager.Tick(d);
        DiscussionWindowPositioner.Tick();
        TextDiscussionWindowDimensionsManager.Tick(d);
        DiscussionWriterManager.Tick(d);
    }

    public void OnGUIDraw()
    {
        TextDiscussionWindowDimensionsManager.OnGUIDraw();
    }

    #region External Events
    public void OnDiscussionWindowAwake(DiscussionTextOnlyNode discussionNode, Transform position)
    {
        DiscussionWindowAnimationManager.PlayEnterAnimation();
        DiscussionWindowPositioner.SetTransformToFollow(position);
        InitializeDiscussionWindow(DiscussionSentencesTextConstants.SentencesText[discussionNode.DisplayedText]);
    }

    public void OnDiscussionWindowSleep()
    {
        DiscussionWorkflowManager.OnDiscussionWindowSleep();
    }

    private void InitializeDiscussionWindow(string fullTextContent)
    {
        DiscussionWorkflowManager.OnDiscussionWindowAwake();
        TextDiscussionWindowDimensionsManager.InitializeDiscussionWindow(fullTextContent);
        var truncatedText = TextDiscussionWindowDimensionsManager.ComputeTrucatedText(fullTextContent);
        DiscussionWriterManager.OnDiscussionTextStartWriting(truncatedText);
    }

    public void ProcessDiscussionContinue()
    {
        InitializeDiscussionWindow(TextDiscussionWindowDimensionsManager.OverlappedOnlyText);
    }

    public IEnumerator PlayDiscussionCloseAnimation()
    {
        return DiscussionWindowAnimationManager.PlayExitAnimation();
    }

    public void OnHeightChange(float newHeight)
    {
        DiscussionWindowDimensionsTransitionManager.OnHeightChange(newHeight);
    }


    #endregion

    #region Internal Events
    public void OnTextFinishedWriting()
    {
        DiscussionWorkflowManager.OnTextFinishedWriting(TextDiscussionWindowDimensionsManager.OverlappedOnlyText);
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

class TextOnlyDiscussionWindowDimensionsManager
{
    private DiscussionWindow DiscussionBaseReference;
    private TextOnlyDiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;
    private Text textAreaText;
    private RectTransform discussionWindowTransform;

    private DiscussionWindowDimensionsComputation discussionWindowDimensionsComputation;
    private int displayedLineNB;
    private int nonOverlappedLineNb;
    private TextGenerator cachedTextGenerator;

    private string overlappedOnlyText;

    public string OverlappedOnlyText { get => overlappedOnlyText; }

    public TextOnlyDiscussionWindowDimensionsManager(DiscussionWindow DiscussionBaseReference, GameObject textAreaObject,
                    GameObject discussionWindowObject, TextOnlyDiscussionWindowDimensionsComponent textOnlyDiscussionWindowDimensionsComponent,
                    DiscussionWindowDimensionsComputation discussionWindowDimensionsComputation)
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
        textAreaText.text = fullTextContent;
        Canvas.ForceUpdateCanvases();
        DiscussionBaseReference.OnHeightChange(discussionWindowDimensionsComputation.GetWindowHeight(this.displayedLineNB));
        CalculateNonOverlappedLineNb();
        textAreaText.text = "";
        Canvas.ForceUpdateCanvases();
    }

    public string ComputeTrucatedText(string fullTextContent)
    {
        var initialSizeDelta = discussionWindowTransform.sizeDelta;

        textAreaText.text = fullTextContent;
        discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, discussionWindowDimensionsComputation.GetWindowHeight(DiscussionWindowDimensionsComponent.MaxLineDisplayed));
        Canvas.ForceUpdateCanvases();

        var truncatedText = textAreaText.text.Substring(0, textAreaText.cachedTextGenerator.characterCountVisible);
        overlappedOnlyText = textAreaText.text.Substring(textAreaText.cachedTextGenerator.characterCountVisible, textAreaText.text.Length - textAreaText.cachedTextGenerator.characterCountVisible);

        textAreaText.text = "";
        discussionWindowTransform.sizeDelta = initialSizeDelta;
        Canvas.ForceUpdateCanvases();

        return truncatedText;
    }
}

#endregion

#region Discussion Window Text Writer
[System.Serializable]
public class DiscussionWriterComponent
{
    public float letterDisplayIntervalTime;
}

class DiscussionWriterManager
{
    private DiscussionWindow DiscussionReference;
    private DiscussionWriterComponent DiscussionWriterComponent;
    private Text textAreaText;

    private string currentDisplayedText;
    private string truncatedTargetText;
    private float timeElapsed;
    private bool isTextWriting;

    public DiscussionWriterManager(DiscussionWindow DiscussionReference, DiscussionWriterComponent discussionWriterComponent, Text textAreaText)
    {
        this.DiscussionReference = DiscussionReference;
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
                while (timeElapsed >= DiscussionWriterComponent.letterDisplayIntervalTime)
                {
                    timeElapsed -= DiscussionWriterComponent.letterDisplayIntervalTime;
                    if (currentDisplayedText.Length < truncatedTargetText.Length)
                    {
                        currentDisplayedText += truncatedTargetText[currentDisplayedText.Length];
                    }
                }
            }
            else
            {
                isTextWriting = false;
                DiscussionReference.OnTextFinishedWriting();
            }
        }

    }

    public void OnDiscussionTextStartWriting(string truncatedTargetText)
    {
        isTextWriting = true;
        timeElapsed = 0f;
        currentDisplayedText = "";
        this.truncatedTargetText = truncatedTargetText;
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
            this.discussionTransform.position = camera.WorldToScreenPoint(worldTransformToFollow.position);
        }
    }
}
#endregion

#region DiscussionWindow Dimensions
[System.Serializable]
public class DiscussionWindowDimensionsComponent
{
    public float Margin;
}

public interface DiscussionWindowDimensionsComputation
{
    float GetCurrentWindowHeight();
    float GetMargin();
    float GetSingleLineHeightWithLineSpace();
    float GetLineSpace();
    float GetWindowHeight(int lineNB);
    float GetTextAreaWidth();
}

class DiscussionWindowDimensionsManager : DiscussionWindowDimensionsComputation
{
    private DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;

    private Text textAreaText;
    private RectTransform discussionWindowTransform;

    public DiscussionWindowDimensionsManager(DiscussionWindowDimensionsComponent discussionWindowDimensionsComponent, GameObject textAreaObject, RectTransform discussionWindowTransform)
    {
        DiscussionWindowDimensionsComponent = discussionWindowDimensionsComponent;

        var textAreaTransform = (RectTransform)textAreaObject.transform;
        this.textAreaText = textAreaObject.GetComponent<Text>();
        this.discussionWindowTransform = discussionWindowTransform;
        var margin = discussionWindowDimensionsComponent.Margin;
        textAreaTransform.offsetMin = new Vector2(margin, margin);
        textAreaTransform.offsetMax = new Vector2(-margin, -margin);
    }

    public float GetCurrentWindowHeight()
    {
        return textAreaText.preferredHeight + (DiscussionWindowDimensionsComponent.Margin * 2);
    }

    public float GetLineSpace()
    {
        return textAreaText.lineSpacing;
    }

    public float GetMargin()
    {
        return DiscussionWindowDimensionsComponent.Margin;
    }

    public float GetSingleLineHeightWithLineSpace()
    {
        return (textAreaText.font.lineHeight + textAreaText.lineSpacing);
    }

    public float GetTextAreaWidth()
    {
        return discussionWindowTransform.sizeDelta.x - (DiscussionWindowDimensionsComponent.Margin * 2);
    }

    public float GetWindowHeight(int lineNb)
    {
        return ((textAreaText.font.lineHeight + textAreaText.lineSpacing) * lineNb) + (DiscussionWindowDimensionsComponent.Margin * 2);
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
}
#endregion

#region Discussion Window Animation
class DiscussionWindowAnimationManager
{
    private const string ENTER_ANIMATION_NAME = "DiscussionWindowEnterAnimation";
    private const string EXIT_ANIMATION_NAME = "DiscussionWindowExitAnimation";

    private Animator DiscussionAnimator;
    private DiscussionEventHandler DiscussionEventHandler;

    private bool isAnimationExitingPlaying;

    public DiscussionWindowAnimationManager(Animator discussionAnimator, DiscussionEventHandler discussionEventHandler)
    {
        DiscussionAnimator = discussionAnimator;
        DiscussionEventHandler = discussionEventHandler;
    }

    public bool IsAnimationExitingPlaying { get => isAnimationExitingPlaying; }

    public void PlayEnterAnimation()
    {
        DiscussionAnimator.Play(ENTER_ANIMATION_NAME);
    }

    public IEnumerator PlayExitAnimation()
    {
        DiscussionAnimator.Play(EXIT_ANIMATION_NAME);
        isAnimationExitingPlaying = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfAnimation(DiscussionAnimator, EXIT_ANIMATION_NAME, 0);
        isAnimationExitingPlaying = false;
    }
}
#endregion