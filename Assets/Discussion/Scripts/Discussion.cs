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

    private void Start()
    {
        var textAreaObject = gameObject.FindChildObjectRecursively(TEXT_AREA_OBJECT_NAME);
        var discussionWindowObject = gameObject.FindChildObjectRecursively(DISCUSSION_WINDOW_OBJECT_NAME);
        DiscussionWindowDimensionsManager = new DiscussionWindowDimensionsManager(this, textAreaObject, discussionWindowObject, DiscussionWindowDimensionsComponent);
        DiscussionWriterManager = new DiscussionWriterManager(DiscussionWriterComponent, textAreaObject.GetComponent<Text>());
    }
    public void Tick(float d)
    {
        if (!IsTextWriting())
        {
            DiscussionWindowDimensionsManager.Tick(d);
        }
        else
        {
            DiscussionWriterManager.Tick(d);
        }

    }

    public void OnGUIDraw()
    {
        DiscussionWindowDimensionsManager.OnGUIDraw();
    }

    #region External Events
    public void OnDiscussionWindowAwake(string fullTextContent)
    {
        DiscussionWindowDimensionsManager.InitializeDiscussionWindow(fullTextContent);
    }
    #endregion

    #region Internal Events
    public void OnTargetWindowHeightReached()
    {
        Canvas.ForceUpdateCanvases();
        var truncatedText = DiscussionWindowDimensionsManager.ComputeTrucatedText();
        DiscussionWriterManager.OnDiscussionTextStartWriting(truncatedText);
    }
    #endregion

    #region Functional Conditions
    private bool IsTextWriting()
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
        if (heightUpdating)
        {
            discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, Mathf.Lerp(discussionWindowTransform.sizeDelta.y, targetWindowheight, d * DiscussionWindowDimensionsComponent.HeightTransitionsSpeed));
            if (Mathf.Abs(targetWindowheight - discussionWindowTransform.sizeDelta.y) <= 0.05)
            {
                discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, targetWindowheight);
                heightUpdating = false;
                DiscussionReference.OnTargetWindowHeightReached();
            }
        }
    }

    private void CalculateTargetWindwHeight()
    {
        heightUpdating = true;
        targetWindowheight = ((textAreaText.font.lineHeight + textAreaText.lineSpacing) * this.displayedLineNB) + (DiscussionWindowDimensionsComponent.Margin * 2);
    }

    public void InitializeDiscussionWindow(string fullTextContent)
    {
        this.displayedLineNB = 1;
        textAreaText.text = fullTextContent;
        CalculateTargetWindwHeight();
        CalculateNonOverlappedLineNb();
        displayedLineNB = Mathf.Min(3, nonOverlappedLineNb);
        CalculateTargetWindwHeight();
    }

    public string ComputeTrucatedText()
    {

        var lines = new List<UILineInfo>();
        textAreaText.cachedTextGenerator.GetLines(lines);
        var truncatedText = "";

        for (var i = 0; i < displayedLineNB; i++)
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

    public void OnDiscussionTextStartWriting(string truncatedTargetText)
    {
        isTextWriting = true;
        timeElapsed = 0f;
        currentDisplayedText = "";
        this.truncatedTargetText = truncatedTargetText;
    }
}
#endregion
