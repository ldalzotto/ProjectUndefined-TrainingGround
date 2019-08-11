using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{
    public class DiscussionText
    {
        private char[] TrimmedCharForSanitaze = new char[] { ' ', '\n' };
        public const string SpecialCharacterImageTemplate = "#00";

        private string initialRawText;
        private string transformedInitialRawText;
        private List<Image> SpecialCharactersImages;

        private string overlappedText;

        private List<string> linedTruncatedText;

        private DiscussionTextWindowDimensions discussionTextWindowDimensions;
        private DiscussionTextPlayerEngine DiscussionTextPlayerEngine;

        public DiscussionText(string initialRawText, DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent,
            TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent, DiscussionHeightChangeListener DiscussionHeightChangeListener, Text textAreaText)
        {

            this.initialRawText = initialRawText;
            this.transformedInitialRawText = Regex.Unescape(this.initialRawText);

            this.SpecialCharactersImages = new List<Image>();
            var keyCharacterRegex = new Regex("@.*?( |$)");
            Match regexMatch = null;
            do
            {
                regexMatch = keyCharacterRegex.Match(this.transformedInitialRawText);
                if (regexMatch.Success)
                {
                    var matchValue = regexMatch.Value.Trim(TrimmedCharForSanitaze);
                    this.transformedInitialRawText = this.transformedInitialRawText.Substring(0, regexMatch.Index)
                        + SpecialCharacterImageTemplate
                        + this.transformedInitialRawText.Substring(regexMatch.Index + matchValue.Length, this.transformedInitialRawText.Length - (regexMatch.Index + matchValue.Length));

                    //TODO -> Mapping
                    var instaciatedImage = MonoBehaviour.Instantiate(PrefabContainer.Instance.InputBaseImage, textAreaText.transform);
                    instaciatedImage.gameObject.SetActive(false);
                    this.SpecialCharactersImages.Add(instaciatedImage);
                }
            } while (regexMatch.Success);

            this.discussionTextWindowDimensions = new DiscussionTextWindowDimensions(DiscussionWindowDimensionsComponent, TextOnlyDiscussionWindowDimensionsComponent);
            this.DiscussionTextPlayerEngine = new DiscussionTextPlayerEngine(TextOnlyDiscussionWindowDimensionsComponent, this.discussionTextWindowDimensions, DiscussionHeightChangeListener);
        }

        #region Data Retrieval
        public List<string> LinedTruncatedText { get => linedTruncatedText; }
        public string OverlappedText { get => overlappedText; }

        public float GetFinalWidth() { return this.discussionTextWindowDimensions.FinalWidth; }
        public float GetMaxWindowHeight(Text textAreaText) { return this.discussionTextWindowDimensions.GetMaxWindowHeight(textAreaText); }
        public float GetWindowHeight(int lineNb, Text textAreaText) { return this.discussionTextWindowDimensions.GetWindowHeight(lineNb, textAreaText); }
        public string GetCurrentDisplayedText() { return this.DiscussionTextPlayerEngine.CurrentDisplayedText; }
        public int GetDisplayedLineNb() { return this.DiscussionTextPlayerEngine.DisplayedLineNb; }
        #endregion

        #region Writing
        public void Increment(Text textAreaText)
        {
            this.DiscussionTextPlayerEngine.Increment(textAreaText, SpecialCharactersImages);
        }
        #endregion

        #region Logical Conditions
        public bool IsAllowedToIncrementEngine()
        {
            return this.DiscussionTextPlayerEngine.IsAllowedToIncrementEngine();
        }
        #endregion

        public void ComputeTruncatedText(RectTransform discussionWindowTransform, Text discussionWindowText)
        {
            var initialSizeDelta = discussionWindowTransform.sizeDelta;

            discussionWindowTransform.sizeDelta = new Vector2(this.discussionTextWindowDimensions.GetMaxWindowWidth(), this.discussionTextWindowDimensions.GetMaxWindowHeight(discussionWindowText));
            this.ForceUpdateTextCachedTextGenerator(this.transformedInitialRawText, discussionWindowText);

            var truncatedText = discussionWindowText.text.Substring(0, discussionWindowText.cachedTextGenerator.characterCountVisible);
            this.overlappedText = discussionWindowText.text.Substring(discussionWindowText.cachedTextGenerator.characterCountVisible, discussionWindowText.text.Length - discussionWindowText.cachedTextGenerator.characterCountVisible);

            truncatedText = truncatedText.Trim(this.TrimmedCharForSanitaze);

            this.ForceUpdateTextCachedTextGenerator(truncatedText, discussionWindowText);

            this.discussionTextWindowDimensions.ComputeFinalDimensions(discussionWindowText);
            discussionWindowTransform.sizeDelta = new Vector2(this.GetFinalWidth(), this.discussionTextWindowDimensions.GetMaxWindowHeight(discussionWindowText));
            this.ForceUpdateTextCachedTextGenerator(truncatedText, discussionWindowText);

            this.linedTruncatedText = new List<string>();
            for (int i = 0; i < discussionWindowText.cachedTextGenerator.lines.Count; i++)
            {
                int startIndex = discussionWindowText.cachedTextGenerator.lines[i].startCharIdx;
                int endIndex = (i == discussionWindowText.cachedTextGenerator.lines.Count - 1) ? discussionWindowText.text.Length
                    : discussionWindowText.cachedTextGenerator.lines[i + 1].startCharIdx;
                int length = endIndex - startIndex;

                string lineToAdd = discussionWindowText.text.Substring(startIndex, length).Trim(this.TrimmedCharForSanitaze);
                this.linedTruncatedText.Add(lineToAdd);
            }

            discussionWindowTransform.sizeDelta = initialSizeDelta;
            this.ForceUpdateTextCachedTextGenerator("", discussionWindowText);

            this.DiscussionTextPlayerEngine.StartWriting(this);
        }

        private void ForceUpdateTextCachedTextGenerator(string textContent, Text textArea)
        {
            textArea.text = textContent;
            Canvas.ForceUpdateCanvases();
        }
    }

    class DiscussionTextWindowDimensions
    {
        private DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;
        private TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent;

        private float finalWidth;

        public DiscussionTextWindowDimensions(DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent,
            TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent)
        {
            this.DiscussionWindowDimensionsComponent = DiscussionWindowDimensionsComponent;
            this.TextOnlyDiscussionWindowDimensionsComponent = TextOnlyDiscussionWindowDimensionsComponent;
        }

        public float FinalWidth { get => finalWidth; }

        public void ComputeFinalDimensions(Text textAreaText)
        {
            this.finalWidth = Mathf.Min(textAreaText.preferredWidth + (DiscussionWindowDimensionsComponent.MarginLeft + DiscussionWindowDimensionsComponent.MarginRight), DiscussionWindowDimensionsComponent.MaxWindowWidth);
        }

        public float GetMaxWindowHeight(Text textAreaText)
        {
            return this.GetWindowHeight(this.TextOnlyDiscussionWindowDimensionsComponent.MaxLineDisplayed, textAreaText);
        }

        public float GetWindowHeight(int lineNb, Text textAreaText)
        {
            return Mathf.Max(DiscussionWindowDimensionsComponent.MinWindowHeight, ((textAreaText.font.lineHeight + textAreaText.lineSpacing) * lineNb) + (DiscussionWindowDimensionsComponent.MarginDown + DiscussionWindowDimensionsComponent.MarginUp));
        }

        public float GetMaxWindowWidth()
        {
            return this.DiscussionWindowDimensionsComponent.MaxWindowWidth;
        }
    }

    class DiscussionTextPlayerEngine
    {
        private TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent;
        private DiscussionTextWindowDimensions DiscussionTextWindowDimensions;
        private DiscussionHeightChangeListener DiscussionHeightChangeListener;

        public DiscussionTextPlayerEngine(TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent,
            DiscussionTextWindowDimensions DiscussionTextWindowDimensions, DiscussionHeightChangeListener DiscussionHeightChangeListener)
        {
            this.TextOnlyDiscussionWindowDimensionsComponent = TextOnlyDiscussionWindowDimensionsComponent;
            this.DiscussionTextWindowDimensions = DiscussionTextWindowDimensions;
            this.DiscussionHeightChangeListener = DiscussionHeightChangeListener;
        }


        private string targetText;
        private string currentDisplayedText;
        private int displayedLineNb;

        private Regex textSpecialImageRegex;

        public string CurrentDisplayedText { get => currentDisplayedText; }
        public int DisplayedLineNb { get => displayedLineNb; }

        public void StartWriting(DiscussionText discussionText)
        {
            this.targetText = String.Join("\n", discussionText.LinedTruncatedText.ToArray());
            this.currentDisplayedText = String.Empty;
            this.displayedLineNb = 1;
            this.textSpecialImageRegex = new Regex(DiscussionText.SpecialCharacterImageTemplate);
        }

        public void Increment(Text textAreaText, List<Image> SpecialCharactersImages)
        {
            if (currentDisplayedText.Length < targetText.Length)
            {
                var charToAdd = targetText[currentDisplayedText.Length];
                if (charToAdd == '#')
                {
                    currentDisplayedText += charToAdd;
                    currentDisplayedText += targetText[currentDisplayedText.Length];
                    currentDisplayedText += targetText[currentDisplayedText.Length];

                    textAreaText.text = currentDisplayedText;
                    Canvas.ForceUpdateCanvases();
                }
                else
                {
                    currentDisplayedText += charToAdd;
                }

                int specialImageMatchCount = 0;
                foreach (Match specialImageMatch in this.textSpecialImageRegex.Matches(currentDisplayedText))
                {
                    if (specialImageMatch.Success)
                    {
                        var drawedCharacters = textAreaText.cachedTextGenerator.GetCharactersArray().ToList();
                        if (drawedCharacters.Count >= specialImageMatch.Index + DiscussionText.SpecialCharacterImageTemplate.Length)
                        {
                            var imagePosition = (drawedCharacters[specialImageMatch.Index].cursorPos + drawedCharacters[specialImageMatch.Index + DiscussionText.SpecialCharacterImageTemplate.Length].cursorPos) / 2f;
                            SpecialCharactersImages[specialImageMatchCount].gameObject.SetActive(true);
                            SpecialCharactersImages[specialImageMatchCount].transform.position = new Vector2(textAreaText.transform.position.x, textAreaText.transform.position.y) + imagePosition;
                            specialImageMatchCount += 1;
                        }
                    }
                }

                var newDisplayedLineNb = Mathf.Min(this.currentDisplayedText.Split('\n').Length, this.TextOnlyDiscussionWindowDimensionsComponent.MaxLineDisplayed);
                if (newDisplayedLineNb != this.displayedLineNb)
                {
                    this.DiscussionHeightChangeListener.OnHeightChange(this.DiscussionTextWindowDimensions.GetWindowHeight(newDisplayedLineNb, textAreaText));
                }
                this.displayedLineNb = newDisplayedLineNb;
            }
        }

        public bool IsAllowedToIncrementEngine()
        {
            return currentDisplayedText.Length != targetText.Length;
        }

    }

    public interface DiscussionHeightChangeListener
    {
        void OnHeightChange(float newHeight);
    }
}
