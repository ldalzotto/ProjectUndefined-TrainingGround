using UnityEngine;
using UnityEngine.UI;

public class TextOnlyDiscussion : MonoBehaviour
{

    private const string CONTINUE_ICON_OBJECT_NAME = "ContinueIcon";
    private const string END_ICON_OBJECT_NAME = "EndIcon";

    public TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent;
    public DiscussionWriterComponent DiscussionWriterComponent;

    private TextOnlyDiscussionWindowDimensionsManager DiscussionWindowDimensionsManager;
    private DiscussionWriterManager DiscussionWriterManager;
    private DiscussionWorkflowManager DiscussionWorkflowManager;

    public void InitializeDependencies(float textAreaMargin)
    {
        var DiscussionBase = GetComponent<DiscussionBase>();

        var textAreaObject = gameObject.FindChildObjectRecursively(DiscussionBase.TEXT_AREA_OBJECT_NAME);
        var discussionWindowObject = gameObject.FindChildObjectRecursively(DiscussionBase.DISCUSSION_WINDOW_OBJECT_NAME);

        DiscussionWindowDimensionsManager = new TextOnlyDiscussionWindowDimensionsManager(DiscussionBase, textAreaObject, discussionWindowObject, TextOnlyDiscussionWindowDimensionsComponent, DiscussionBase.GetDiscussionWindowDimensionsComputation());
        DiscussionWriterManager = new DiscussionWriterManager(this, DiscussionWriterComponent, textAreaObject.GetComponent<Text>());
        DiscussionWorkflowManager = new DiscussionWorkflowManager(gameObject.FindChildObjectRecursively(CONTINUE_ICON_OBJECT_NAME), gameObject.FindChildObjectRecursively(END_ICON_OBJECT_NAME));
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
    public void OnDiscussionWindowAwake(string fullTextContent, Transform anchoredDiscussionWorldTransform)
    {
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
public class TextOnlyDiscussionWindowDimensionsComponent
{
    public int MaxLineDisplayed;
}

class TextOnlyDiscussionWindowDimensionsManager
{
    private DiscussionBase DiscussionBaseReference;
    private TextOnlyDiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;
    private Text textAreaText;
    private RectTransform discussionWindowTransform;

    private DiscussionWindowDimensionsComputation discussionWindowDimensionsComputation;
    private int displayedLineNB;
    private int nonOverlappedLineNb;
    private TextGenerator cachedTextGenerator;

    private string overlappedOnlyText;

    public string OverlappedOnlyText { get => overlappedOnlyText; }

    public TextOnlyDiscussionWindowDimensionsManager(DiscussionBase DiscussionBaseReference, GameObject textAreaObject,
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
    private TextOnlyDiscussion DiscussionReference;
    private DiscussionWriterComponent DiscussionWriterComponent;
    private Text textAreaText;

    private string currentDisplayedText;
    private string truncatedTargetText;
    private float timeElapsed;
    private bool isTextWriting;

    public DiscussionWriterManager(TextOnlyDiscussion DiscussionReference, DiscussionWriterComponent discussionWriterComponent, Text textAreaText)
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

