using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceDiscussion : MonoBehaviour
{

    private const string SELECTION_AREA_EFFECT_OBJECT_NAME = "SelectionAreaEffect";

    public ChoiceDiscussionDimensionManagerComponent ChoiceDiscussionDimensionManagerComponent;
    public ChoiceVisualFeedbackManagerComponent ChoiceVisualFeedbackManagerComponent;

    private FullChoiceTextParserManager FullChoiceTextParserManager;
    private ChoiceDiscussionTextWriterManager ChoiceDiscussionTextWriterManager;
    private ChoiceDiscussionDimensionManager ChoiceDiscussionDimensionManager;
    private ChoiceVisualFeedbackManager ChoiceVisualFeedbackManager;
    private DiscussionChoiceSelectionInputManager DiscussionChoiceSelectionInputManager;

    public void InitializeDependencies()
    {
        #region External Dependencies
        var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        var discussionWindow = gameObject.FindChildObjectRecursively(DiscussionBase.DISCUSSION_WINDOW_OBJECT_NAME);
        var textAreaObject = gameObject.FindChildObjectRecursively(DiscussionBase.TEXT_AREA_OBJECT_NAME);
        var textAreaText = textAreaObject.GetComponent<Text>();

        var DiscussionBaseReference = GetComponent<DiscussionBase>();
        FullChoiceTextParserManager = new FullChoiceTextParserManager(textAreaText, (RectTransform)textAreaObject.transform);
        ChoiceDiscussionTextWriterManager = new ChoiceDiscussionTextWriterManager(textAreaText);
        ChoiceDiscussionDimensionManager = new ChoiceDiscussionDimensionManager(DiscussionBaseReference, this, ChoiceDiscussionDimensionManagerComponent, textAreaText);
        ChoiceVisualFeedbackManager = new ChoiceVisualFeedbackManager(ChoiceVisualFeedbackManagerComponent, gameObject.FindChildObjectRecursively(SELECTION_AREA_EFFECT_OBJECT_NAME), (RectTransform)discussionWindow.transform, DiscussionBaseReference.GetDiscussionWindowDimensionsComputation());
        DiscussionChoiceSelectionInputManager = new DiscussionChoiceSelectionInputManager(this, gameInputManager);
    }

    public void Tick(float d)
    {
        DiscussionChoiceSelectionInputManager.Tick();
        ChoiceVisualFeedbackManager.Tick(d);
    }

    public void OnGUIDraw()
    {

    }

    #region External Events
    public void OnDiscussionWindowAwake(DiscussionWindowChoiceInput discussionWindowChoiceInput)
    {
        FullChoiceTextParserManager.ParseFullChoiceText(discussionWindowChoiceInput);
        ChoiceDiscussionDimensionManager.InitializeDisplay(FullChoiceTextParserManager.ParsedChoiceDiscussion);
        ChoiceVisualFeedbackManager.OnDiscussionWindowAwake();
    }
    public void OnDiscussionWindowSleep()
    {
        ChoiceVisualFeedbackManager.OnDiscussionWindowSleep();
    }
    #endregion

    #region Internal Events
    public void OnDisplayedTextChange(string newDisplayedText)
    {
        ChoiceDiscussionTextWriterManager.SetDisplayedText(newDisplayedText);
    }
    public void OnSelectionChange(int selectedChoiceIndex, Vector2 anchoredPosition, float height)
    {
        ChoiceVisualFeedbackManager.OnSelectionChange(selectedChoiceIndex, anchoredPosition, height);
    }
    public void OnSelectChoiceUp()
    {
        ChoiceDiscussionDimensionManager.OnSelectChoiceUp(ChoiceVisualFeedbackManager.SelectedChoiceIndex, FullChoiceTextParserManager.ParsedChoiceDiscussion);
    }

    public void OnSelectChoiceDown()
    {
        ChoiceDiscussionDimensionManager.OnSelectChoiceDown(ChoiceVisualFeedbackManager.SelectedChoiceIndex, FullChoiceTextParserManager.ParsedChoiceDiscussion);
    }
    #endregion

}

#region Choice text parser
class FullChoiceTextParserManager
{
    private const string CHOICE_STRING_DELIMITER = "##";

    private Text textArea;
    private RectTransform discussionWindowTransform;

    private ParsedChoiceDiscussion parsedChoiceDiscussion;

    public FullChoiceTextParserManager(Text textArea, RectTransform discussionWindowTransform)
    {
        this.textArea = textArea;
        this.discussionWindowTransform = discussionWindowTransform;
    }

    internal ParsedChoiceDiscussion ParsedChoiceDiscussion { get => parsedChoiceDiscussion; }

    public void ParseFullChoiceText(DiscussionWindowChoiceInput DiscussionWindowChoiceInput)
    {
        var parsedFullText = DiscussionChoiceTextConstants.ChoiceIntroductionTexts[DiscussionWindowChoiceInput.IntroductionDiscussionId] + StringConstants.RETURN;

        foreach (var choiceTextId in DiscussionWindowChoiceInput.ChoicesTextId)
        {
            parsedFullText += CHOICE_STRING_DELIMITER + StringConstants.RETURN;
            parsedFullText += DiscussionChoiceTextConstants.ChoiceTexts[choiceTextId] + StringConstants.RETURN;
        }

        var parsedLines = FullTextLineGenerator(parsedFullText);

        ParsedTextWithLines parsedIntroduction = null;
        List<ParsedTextWithLines> parsedChoices = new List<ParsedTextWithLines>();

        string parsedTextAccumulator = "";
        int startIndexTracker = 0;

        for (var i = 0; i < parsedLines.Count; i++)
        {
            if (parsedLines[i] == CHOICE_STRING_DELIMITER + StringConstants.RETURN)
            {
                if (parsedIntroduction == null)
                {
                    parsedIntroduction = new ParsedTextWithLines(parsedTextAccumulator, i - startIndexTracker);
                }
                else
                {
                    parsedChoices.Add(new ParsedTextWithLines(parsedTextAccumulator, i - startIndexTracker));
                }
                startIndexTracker = i + 1;
                parsedTextAccumulator = "";
            }
            else
            {
                parsedTextAccumulator += parsedLines[i];
            }
        }
        parsedChoices.Add(new ParsedTextWithLines(parsedTextAccumulator, parsedLines.Count - startIndexTracker));

        parsedChoiceDiscussion = new ParsedChoiceDiscussion(parsedIntroduction, parsedChoices);
    }

    private List<string> FullTextLineGenerator(string parsedFullText)
    {
        var oldText = textArea.text;
        Vector2 oldWindowDimensions = discussionWindowTransform.sizeDelta;

        discussionWindowTransform.sizeDelta = new Vector2(discussionWindowTransform.sizeDelta.x, 15000);
        textArea.text = parsedFullText;
        Canvas.ForceUpdateCanvases();


        var fullTextLinesArray = textArea.cachedTextGenerator.GetLinesArray();

        textArea.text = oldText;
        discussionWindowTransform.sizeDelta = oldWindowDimensions;
        Canvas.ForceUpdateCanvases();

        List<string> fullTextLines = new List<string>();

        int currentStartIndex = 0;
        int nextStartIndex = 0;
        for (var i = 0; i < fullTextLinesArray.Length - 1; i++)
        {
            nextStartIndex = fullTextLinesArray[i + 1].startCharIdx;
            var lineToAdd = parsedFullText.Substring(currentStartIndex, nextStartIndex - currentStartIndex);
            fullTextLines.Add(lineToAdd);
            currentStartIndex = nextStartIndex;
        }

        return fullTextLines;
    }
}

class ParsedChoiceDiscussion
{
    private ParsedTextWithLines introduction;
    private List<ParsedTextWithLines> choices;

    public ParsedChoiceDiscussion(ParsedTextWithLines introduction, List<ParsedTextWithLines> choices)
    {
        this.introduction = introduction;
        this.choices = choices;
    }

    internal ParsedTextWithLines Introduction { get => introduction; }
    internal List<ParsedTextWithLines> Choices { get => choices; }
}

class ParsedTextWithLines
{
    private string text;
    private int nbLines;

    public ParsedTextWithLines(string text, int nbLines)
    {
        this.text = text;
        this.nbLines = nbLines;
    }

    public string Text { get => text; }
    public int NbLines { get => nbLines; }
}
#endregion

#region Choice Discussion Window Dimension Manager
[System.Serializable]
public class ChoiceDiscussionDimensionManagerComponent
{
    public int MaxLineNb;
}

class ChoiceDiscussionDimensionManager
{
    private DiscussionBase DiscussionBaseReference;
    private ChoiceDiscussion ChoiceDiscussionReference;
    private ChoiceDiscussionDimensionManagerComponent ChoiceDiscussionDimensionManagerComponent;
    private DiscussionWindowDimensionsComputation DiscussionWindowDimensionsComputation;
    private Text textArea;

    public ChoiceDiscussionDimensionManager(DiscussionBase DiscussionBaseReference, ChoiceDiscussion ChoiceDiscussionReference, ChoiceDiscussionDimensionManagerComponent choiceDiscussionDimensionManagerComponent,
             Text textArea)
    {
        this.DiscussionBaseReference = DiscussionBaseReference;
        this.ChoiceDiscussionReference = ChoiceDiscussionReference;
        ChoiceDiscussionDimensionManagerComponent = choiceDiscussionDimensionManagerComponent;
        DiscussionWindowDimensionsComputation = DiscussionBaseReference.GetDiscussionWindowDimensionsComputation();
        this.textArea = textArea;
    }


    private List<int> displayedChoices = new List<int>();

    public void InitializeDisplay(ParsedChoiceDiscussion ParsedChoiceDiscussion)
    {
        var linesDedicatedToChoices = ChoiceDiscussionDimensionManagerComponent.MaxLineNb - ParsedChoiceDiscussion.Introduction.NbLines;

        List<ParsedTextWithLines> disaplyedChoices = new List<ParsedTextWithLines>();
        var displayedChoicesTotalLineNB = 0;
        foreach (var parsedChoice in ParsedChoiceDiscussion.Choices)
        {
            if (displayedChoicesTotalLineNB + parsedChoice.NbLines <= linesDedicatedToChoices)
            {
                disaplyedChoices.Add(parsedChoice);
                displayedChoicesTotalLineNB += parsedChoice.NbLines;
                displayedChoices.Add(ParsedChoiceDiscussion.Choices.IndexOf(parsedChoice));
            }
        }

        OnDisplayedChoicesChange(ParsedChoiceDiscussion, displayedChoicesTotalLineNB);

        ChoiceDiscussionReference.OnSelectionChange(0, new Vector2(0, -DiscussionWindowDimensionsComputation.GetSingleLineHeightWithLineSpace() * ParsedChoiceDiscussion.Introduction.NbLines),
            DiscussionWindowDimensionsComputation.GetSingleLineHeightWithLineSpace() * ParsedChoiceDiscussion.Choices[0].NbLines);

    }

    private void PocessSelectionChange(ParsedChoiceDiscussion ParsedChoiceDiscussion, int newSelectedChoice, int oldSelectedChoice)
    {
        if (displayedChoices.Contains(newSelectedChoice))
        {
            OnSelectionChange(ParsedChoiceDiscussion, newSelectedChoice);
        }
        else if (ParsedChoiceDiscussion.Choices.ConvertAll(choice => ParsedChoiceDiscussion.Choices.IndexOf(choice)).Contains(newSelectedChoice))
        {
            var disaplyedChoiceTotalLineNB = 0;
            var deltaIndex = newSelectedChoice - oldSelectedChoice;
            var updateDisplayedChoices = new List<int>();
            foreach (var displayedChoice in displayedChoices)
            {
                var newChoiceIndex = displayedChoice + deltaIndex;
                var nbLinesOfChoiceToAdd = ParsedChoiceDiscussion.Choices[newChoiceIndex].NbLines;

                if (disaplyedChoiceTotalLineNB + nbLinesOfChoiceToAdd <= ChoiceDiscussionDimensionManagerComponent.MaxLineNb - ParsedChoiceDiscussion.Introduction.NbLines)
                {
                    updateDisplayedChoices.Add(newChoiceIndex);
                    disaplyedChoiceTotalLineNB += nbLinesOfChoiceToAdd;
                }
            }
            displayedChoices = updateDisplayedChoices;

            OnDisplayedChoicesChange(ParsedChoiceDiscussion, disaplyedChoiceTotalLineNB);
            OnSelectionChange(ParsedChoiceDiscussion, newSelectedChoice);
        }
    }

    #region Choice Dimensions Events
    private void OnDisplayedChoicesChange(ParsedChoiceDiscussion ParsedChoiceDiscussion, int displayedChoicesTotalLineNB)
    {
        DiscussionBaseReference.OnHeightChange(DiscussionWindowDimensionsComputation.GetWindowHeight(displayedChoicesTotalLineNB + ParsedChoiceDiscussion.Introduction.NbLines));

        var textToDisplay = ParsedChoiceDiscussion.Introduction.Text;
        foreach (var displayedChoice in displayedChoices)
        {
            textToDisplay += ParsedChoiceDiscussion.Choices[displayedChoice].Text;
        }
        ChoiceDiscussionReference.OnDisplayedTextChange(textToDisplay);
    }

    public void OnSelectChoiceUp(int oldSelectedChoice, ParsedChoiceDiscussion ParsedChoiceDiscussion)
    {
        var newSelectedChoice = oldSelectedChoice - 1;
        PocessSelectionChange(ParsedChoiceDiscussion, newSelectedChoice, oldSelectedChoice);
    }

    public void OnSelectChoiceDown(int oldSelectedChoice, ParsedChoiceDiscussion ParsedChoiceDiscussion)
    {
        var newSelectedChoice = oldSelectedChoice + 1;
        PocessSelectionChange(ParsedChoiceDiscussion, newSelectedChoice, oldSelectedChoice);
    }

    private void OnSelectionChange(ParsedChoiceDiscussion ParsedChoiceDiscussion, int newSelectedChoice)
    {
        var upLineNBDelta = 0;
        foreach (var displayedChoice in displayedChoices)
        {
            if (displayedChoice != newSelectedChoice)
            {
                upLineNBDelta += ParsedChoiceDiscussion.Choices[displayedChoice].NbLines;
            }
            else
            {
                break;
            }
        }
        ChoiceDiscussionReference.OnSelectionChange(newSelectedChoice, new Vector2(0, -DiscussionWindowDimensionsComputation.GetSingleLineHeightWithLineSpace() * (ParsedChoiceDiscussion.Introduction.NbLines + upLineNBDelta)),
            DiscussionWindowDimensionsComputation.GetSingleLineHeightWithLineSpace() * ParsedChoiceDiscussion.Choices[newSelectedChoice].NbLines);
    }
    #endregion
}
#endregion

#region Discussion Window Text Writer
class ChoiceDiscussionTextWriterManager
{
    private Text textArea;

    public ChoiceDiscussionTextWriterManager(Text textArea)
    {
        this.textArea = textArea;
    }

    public void SetDisplayedText(string textToDisplay)
    {
        textArea.text = textToDisplay;
    }

}
#endregion

#region Discussion Choice Selection Visual Feedback
[System.Serializable]
public class ChoiceVisualFeedbackManagerComponent
{
    public float TransitionSpeed;
}

class ChoiceVisualFeedbackManager
{
    private ChoiceVisualFeedbackManagerComponent ChoiceVisualFeedbackManagerComponent;
    private GameObject selectedAreaEffect;
    private RectTransform selectedAreaEffectTransform;
    private RectTransform discussionWindowTransform;
    private DiscussionWindowDimensionsComputation DiscussionWindowDimensionsComputation;

    public ChoiceVisualFeedbackManager(ChoiceVisualFeedbackManagerComponent ChoiceVisualFeedbackManagerComponent, GameObject selectedAreaEffect, RectTransform discussionWindowTransform, DiscussionWindowDimensionsComputation DiscussionWindowDimensionsComputation)
    {
        this.ChoiceVisualFeedbackManagerComponent = ChoiceVisualFeedbackManagerComponent;
        this.selectedAreaEffect = selectedAreaEffect;
        this.discussionWindowTransform = discussionWindowTransform;
        this.selectedAreaEffectTransform = (RectTransform)this.selectedAreaEffect.transform;
        this.DiscussionWindowDimensionsComputation = DiscussionWindowDimensionsComputation;

        this.selectedAreaEffect.SetActive(false);
    }

    public void OnDiscussionWindowAwake()
    {
        selectedAreaEffect.SetActive(true);
    }

    public void OnDiscussionWindowSleep()
    {
        selectedAreaEffect.SetActive(false);
    }

    //TODO -> Identify with choice ID instead of index
    private int selectedChoiceIndex;

    private Vector2 targetAnchoredPosition;
    private float targetHeight;

    private bool isMoving;

    public int SelectedChoiceIndex { get => selectedChoiceIndex; }

    public void OnSelectionChange(int selectedChoiceIndex, Vector2 anchoredPosition, float height)
    {
        this.selectedChoiceIndex = selectedChoiceIndex;
        this.targetAnchoredPosition = anchoredPosition;
        this.targetHeight = height;
        isMoving = true;
    }

    public void Tick(float d)
    {
        if (isMoving)
        {
            selectedAreaEffectTransform.anchoredPosition = Vector2.Lerp(selectedAreaEffectTransform.anchoredPosition, targetAnchoredPosition, d * ChoiceVisualFeedbackManagerComponent.TransitionSpeed);
            selectedAreaEffectTransform.sizeDelta = new Vector2(DiscussionWindowDimensionsComputation.GetTextAreaWidth(), targetHeight);

            if (Vector2.Distance(targetAnchoredPosition, selectedAreaEffectTransform.anchoredPosition) <= 0.05)
            {
                isMoving = false;
                selectedAreaEffectTransform.anchoredPosition = targetAnchoredPosition;
            }
        }

    }

}
#endregion

#region Discussion Choice Selection Input
class DiscussionChoiceSelectionInputManager
{
    private ChoiceDiscussion ChoiceDiscussionReference;
    private GameInputManager GameInputManager;

    public DiscussionChoiceSelectionInputManager(ChoiceDiscussion choiceDiscussionReference, GameInputManager gameInputManager)
    {
        ChoiceDiscussionReference = choiceDiscussionReference;
        GameInputManager = gameInputManager;
    }

    private bool isPressed;

    public void Tick()
    {
        var leftStickAxis = GameInputManager.CurrentInput.LocomotionAxis();

        if (leftStickAxis.z >= 0.5)
        {
            if (!isPressed)
            {
                ChoiceDiscussionReference.OnSelectChoiceUp();
            }
            isPressed = true;
        }
        else if (leftStickAxis.z <= -0.5)
        {
            if (!isPressed)
            {
                ChoiceDiscussionReference.OnSelectChoiceDown();
            }
            isPressed = true;
        }
        else
        {
            isPressed = false;
        }

    }
}
#endregion