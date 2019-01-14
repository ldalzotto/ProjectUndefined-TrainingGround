using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Discussion : MonoBehaviour
{

    private const string TEXT_AREA_OBJECT_NAME = "TextArea";
    private const string DISCUSSION_WINDOW_OBJECT_NAME = "DiscussionWindow";
    private const string CONTINUE_ICON_OBJECT_NAME = "ContinueIcon";
    private const string END_ICON_OBJECT_NAME = "EndIcon";

    public DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;
    public DiscussionWriterComponent DiscussionWriterComponent;

    private DiscussionWindowDimensionsManager DiscussionWindowDimensionsManager;
    private DiscussionWriterManager DiscussionWriterManager;
    private DiscussionWorkflowManager DiscussionWorkflowManager;
    private DiscussionWindowAnimationManager DiscussionWindowAnimationManager;

    private void Start()
    {
        var discussionAnimator = GetComponent<Animator>();
        var textAreaObject = gameObject.FindChildObjectRecursively(TEXT_AREA_OBJECT_NAME);
        var discussionWindowObject = gameObject.FindChildObjectRecursively(DISCUSSION_WINDOW_OBJECT_NAME);

        #region External Event hanlder
        var DiscussionEventHandler = GameObject.FindObjectOfType<DiscussionEventHandler>();
        #endregion

        DiscussionWindowDimensionsManager = new DiscussionWindowDimensionsManager(this, textAreaObject, discussionWindowObject, DiscussionWindowDimensionsComponent);
        DiscussionWriterManager = new DiscussionWriterManager(this, DiscussionWriterComponent, textAreaObject.GetComponent<Text>());
        DiscussionWorkflowManager = new DiscussionWorkflowManager(gameObject.FindChildObjectRecursively(CONTINUE_ICON_OBJECT_NAME), gameObject.FindChildObjectRecursively(END_ICON_OBJECT_NAME));
        DiscussionWindowAnimationManager = new DiscussionWindowAnimationManager(discussionAnimator, DiscussionEventHandler);
    }
    public void Tick(float d)
    {
        DiscussionWindowDimensionsManager.Tick(d);
        DiscussionWriterManager.Tick(d);
    }

    public void OnGUIDraw()
    {
        DiscussionWindowDimensionsManager.OnGUIDraw();
    }

    #region External Events
    public void OnDiscussionWindowAwake(string fullTextContent)
    {
        DiscussionWindowAnimationManager.PlayEnterAnimation();
        InitializeDiscussionWindow(fullTextContent);
    }

    public void OnDiscussionWindowSleep()
    {
        DiscussionWorkflowManager.OnDiscussionWindowSleep();
    }

    private void InitializeDiscussionWindow(string fullTextContent)
    {
        DiscussionWorkflowManager.OnDiscussionWindowAwake();
        DiscussionWindowDimensionsManager.InitializeDiscussionWindow(fullTextContent);
        var truncatedText = DiscussionWindowDimensionsManager.ComputeTrucatedText(fullTextContent);
        DiscussionWriterManager.OnDiscussionTextStartWriting(truncatedText);
    }

    public void ProcessDiscussionContinue()
    {
        InitializeDiscussionWindow(DiscussionWindowDimensionsManager.OverlappedOnlyText);
    }

    public void ProcessDiscussionEnd()
    {
        StartCoroutine(DiscussionWindowAnimationManager.PlayExitAnimation());
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
    #endregion
}

#region Discussion Window Dimensions
[System.Serializable]
public class DiscussionWindowDimensionsComponent
{
    public float Margin;
    public int MaxLineDisplayed;
    public float HeightTransitionsSpeed;
}

class DiscussionWindowDimensionsManager
{
    private Discussion DiscussionReference;
    private DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;
    private Text textAreaText;
    private RectTransform discussionWindowTransform;

    private int displayedLineNB;
    private int nonOverlappedLineNb;
    private TextGenerator cachedTextGenerator;

    private float targetWindowheight;
    private bool heightUpdating;

    private string overlappedOnlyText;

    public string OverlappedOnlyText { get => overlappedOnlyText; }

    public DiscussionWindowDimensionsManager(Discussion DiscussionReference, GameObject textAreaObject, GameObject discussionWindowObject, DiscussionWindowDimensionsComponent discussionWindowDimensionsComponent)
    {
        this.displayedLineNB = 1;
        this.DiscussionReference = DiscussionReference;
        this.textAreaText = textAreaObject.GetComponent<Text>();
        this.discussionWindowTransform = (RectTransform)discussionWindowObject.transform;
        this.DiscussionWindowDimensionsComponent = discussionWindowDimensionsComponent;
        var textAreaTransform = (RectTransform)textAreaObject.transform;
        var margin = discussionWindowDimensionsComponent.Margin;
        textAreaTransform.offsetMin = new Vector2(margin, margin);
        textAreaTransform.offsetMax = new Vector2(-margin, -margin);
    }

    public void OnGUIDraw()
    {
        cachedTextGenerator = textAreaText.cachedTextGenerator;
    }

    private void CalculateNonOverlappedLineNb()
    {
        this.nonOverlappedLineNb = Mathf.FloorToInt((textAreaText.preferredHeight) / (textAreaText.font.lineHeight + textAreaText.lineSpacing));
    }

    public void Tick(float d)
    {
        if (IsCurrentTextOverlaps())
        {
            this.displayedLineNB = Mathf.Min(this.displayedLineNB + 1, DiscussionWindowDimensionsComponent.MaxLineDisplayed);
            heightUpdating = true;
            targetWindowheight = CalculateWindowHeight(this.displayedLineNB);
        }

        if (heightUpdating)
        {
            discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, Mathf.Lerp(discussionWindowTransform.sizeDelta.y, targetWindowheight, d * DiscussionWindowDimensionsComponent.HeightTransitionsSpeed));
            if (Mathf.Abs(targetWindowheight - discussionWindowTransform.sizeDelta.y) <= 0.05)
            {
                discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, targetWindowheight);
                heightUpdating = false;
            }
        }
    }

    private float CalculateWindowHeight(int lineNb)
    {
        return ((textAreaText.font.lineHeight + textAreaText.lineSpacing) * lineNb) + (DiscussionWindowDimensionsComponent.Margin * 2);
    }

    private bool IsCurrentTextOverlaps()
    {
        return (textAreaText.preferredHeight + (DiscussionWindowDimensionsComponent.Margin * 2) > CalculateWindowHeight(this.displayedLineNB));
    }

    public void InitializeDiscussionWindow(string fullTextContent)
    {
        this.displayedLineNB = 1;
        textAreaText.text = fullTextContent;
        Canvas.ForceUpdateCanvases();
        heightUpdating = true;
        targetWindowheight = CalculateWindowHeight(this.displayedLineNB);
        CalculateNonOverlappedLineNb();
        textAreaText.text = "";
        Canvas.ForceUpdateCanvases();
    }

    public string ComputeTrucatedText(string fullTextContent)
    {
        var initialSizeDelta = discussionWindowTransform.sizeDelta;

        textAreaText.text = fullTextContent;
        discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, CalculateWindowHeight(DiscussionWindowDimensionsComponent.MaxLineDisplayed));
        Canvas.ForceUpdateCanvases();

        var truncatedText = textAreaText.text.Substring(0, textAreaText.cachedTextGenerator.characterCountVisible);
        overlappedOnlyText = textAreaText.text.Substring(textAreaText.cachedTextGenerator.characterCountVisible, textAreaText.text.Length - textAreaText.cachedTextGenerator.characterCountVisible);

        textAreaText.text = "";
        discussionWindowTransform.sizeDelta = initialSizeDelta;
        Canvas.ForceUpdateCanvases();

        return truncatedText;
    }

    private void SliceTextLine(List<UILineInfo> lines, int i, out int beginIndex, out int endIndex)
    {
        beginIndex = lines[i].startCharIdx;
        endIndex = 0;
        if (i == lines.Count - 1)
        {
            endIndex = textAreaText.text.Length;
        }
        else
        {
            endIndex = lines[i + 1].startCharIdx;
        }
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
    private Discussion DiscussionReference;
    private DiscussionWriterComponent DiscussionWriterComponent;
    private Text textAreaText;

    private string currentDisplayedText;
    private string truncatedTargetText;
    private float timeElapsed;
    private bool isTextWriting;

    public DiscussionWriterManager(Discussion DiscussionReference, DiscussionWriterComponent discussionWriterComponent, Text textAreaText)
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
                    currentDisplayedText += truncatedTargetText[currentDisplayedText.Length];
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

    internal void OnDiscussionWindowSleep()
    {
        ResetState();
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

    public DiscussionWindowAnimationManager(Animator discussionAnimator, DiscussionEventHandler discussionEventHandler)
    {
        DiscussionAnimator = discussionAnimator;
        DiscussionEventHandler = discussionEventHandler;
    }

    public void PlayEnterAnimation()
    {
        DiscussionAnimator.Play(ENTER_ANIMATION_NAME);
    }

    public IEnumerator PlayExitAnimation()
    {
        DiscussionAnimator.Play(EXIT_ANIMATION_NAME);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfAnimation(DiscussionAnimator, EXIT_ANIMATION_NAME, 0);
        DiscussionEventHandler.OnDiscussionWindowSleep();
    }
}
#endregion