using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{
    public class DiscussionText
    {
        private char[] TrimmedCharForSanitaze = new char[] { ' ', '\n' };

        private string initialRawText;
        private string unescapedInitialRawText;

        private string overlappedText;

        private List<string> linedTruncatedText;

        private DiscussionTextWindowDimensions discussionTextWindowDimensions;
        private DiscussionTextPlayerEngine DiscussionTextPlayerEngine;

        public DiscussionText(string initialRawText, DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent,
            TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent, DiscussionHeightChangeListener DiscussionHeightChangeListener)
        {
            this.initialRawText = initialRawText;
            this.unescapedInitialRawText = Regex.Unescape(this.initialRawText);

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
            this.DiscussionTextPlayerEngine.Increment(textAreaText);
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
            this.ForceUpdateTextCachedTextGenerator(this.unescapedInitialRawText, discussionWindowText);

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
                this.linedTruncatedText.Add(discussionWindowText.text.Substring(startIndex, length).Trim(this.TrimmedCharForSanitaze));
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


        public string CurrentDisplayedText { get => currentDisplayedText; }
        public int DisplayedLineNb { get => displayedLineNb; }

        public void StartWriting(DiscussionText discussionText)
        {
            this.targetText = String.Join("\n", discussionText.LinedTruncatedText.ToArray());
            this.currentDisplayedText = String.Empty;
            this.displayedLineNb = 1;
        }

        public void Increment(Text textAreaText)
        {
            if (currentDisplayedText.Length < targetText.Length)
            {
                currentDisplayedText += targetText[currentDisplayedText.Length];
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
