using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Discussion : MonoBehaviour
{

    private const string TEXT_AREA_OBJECT_NAME = "TextArea";
    private const string DISCUSSION_WINDOW_OBJECT_NAME = "DiscussionWindow";

    public DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;
    public DiscussionWriterComponent DiscussionWriterComponent;

    private DiscussionWindowDimensionsManager DiscussionWindowDimensionsManager;
    private DiscussionWriterManager DiscussionWriterManager;
    private DiscussionWindowAnimationManager DiscussionWindowAnimationManager;

    private void Start()
    {
        var discussionAnimator = GetComponent<Animator>();
        var textAreaObject = gameObject.FindChildObjectRecursively(TEXT_AREA_OBJECT_NAME);
        var discussionWindowObject = gameObject.FindChildObjectRecursively(DISCUSSION_WINDOW_OBJECT_NAME);
        DiscussionWindowDimensionsManager = new DiscussionWindowDimensionsManager(this, textAreaObject, discussionWindowObject, DiscussionWindowDimensionsComponent);
        DiscussionWriterManager = new DiscussionWriterManager(DiscussionWriterComponent, textAreaObject.GetComponent<Text>());
        DiscussionWindowAnimationManager = new DiscussionWindowAnimationManager(discussionAnimator);
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
        DiscussionWindowDimensionsManager.InitializeDiscussionWindow(fullTextContent);
        var truncatedText = DiscussionWindowDimensionsManager.ComputeTrucatedText(fullTextContent);
        DiscussionWriterManager.OnDiscussionTextStartWriting(truncatedText);
    }
    #endregion

    #region Internal Events
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
            targetWindowheight = CalculateTargetWindwHeight();
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

    private float CalculateTargetWindwHeight()
    {
        return ((textAreaText.font.lineHeight + textAreaText.lineSpacing) * this.displayedLineNB) + (DiscussionWindowDimensionsComponent.Margin * 2);
    }

    private bool IsCurrentTextOverlaps()
    {
        return (textAreaText.preferredHeight + (DiscussionWindowDimensionsComponent.Margin * 2) > CalculateTargetWindwHeight());
    }

    public void InitializeDiscussionWindow(string fullTextContent)
    {
        this.displayedLineNB = 1;
        textAreaText.text = fullTextContent;
        Canvas.ForceUpdateCanvases();
        heightUpdating = true;
        targetWindowheight = CalculateTargetWindwHeight();
        CalculateNonOverlappedLineNb();
        textAreaText.text = "";
        Canvas.ForceUpdateCanvases();
    }

    public string ComputeTrucatedText(string fullTextContent)
    {
        textAreaText.text = fullTextContent;
        Canvas.ForceUpdateCanvases();

        var lines = new List<UILineInfo>();
        textAreaText.cachedTextGenerator.GetLines(lines);
        var truncatedText = "";
        overlappedOnlyText = "";

        for (var i = 0; i < DiscussionWindowDimensionsComponent.MaxLineDisplayed; i++)
        {
            if (i < lines.Count)
            {
                var beginIndex = lines[i].startCharIdx;
                int endIndex = 0;
                if (i == lines.Count - 1)
                {
                    endIndex = textAreaText.text.Length;
                }
                else
                {
                    endIndex = lines[i + 1].startCharIdx;
                }
                truncatedText += textAreaText.text.Substring(beginIndex, (endIndex - beginIndex));
            }
        }

        textAreaText.text = "";
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
    private DiscussionWriterComponent DiscussionWriterComponent;
    private Text textAreaText;

    private string currentDisplayedText;
    private string truncatedTargetText;
    private float timeElapsed;
    private bool isTextWriting;

    public DiscussionWriterManager(DiscussionWriterComponent discussionWriterComponent, Text textAreaText)
    {
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
            }
        }

    }

    public void OnDiscussionTextStartWriting(string truncatedTargetText)
    {
        Debug.Log(truncatedTargetText);
        isTextWriting = true;
        timeElapsed = 0f;
        currentDisplayedText = "";
        this.truncatedTargetText = truncatedTargetText;
    }
}
#endregion

#region Discussion Window Animation
class DiscussionWindowAnimationManager
{
    private const string ENTER_ANIMATION_NAME = "DiscussionWindowEnterAnimation";

    private Animator DiscussionAnimator;

    public DiscussionWindowAnimationManager(Animator discussionAnimator)
    {
        DiscussionAnimator = discussionAnimator;
    }

    public void PlayEnterAnimation()
    {
        DiscussionAnimator.Play(ENTER_ANIMATION_NAME);
    }
}
#endregion