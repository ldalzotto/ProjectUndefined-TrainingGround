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
        public const string SpecialCharacterImageTemplate = "#0";

        private string initialRawText;
        private string transformedInitialRawText;

        private string overlappedText;

        private List<string> linedTruncatedText;
        private List<Image> SpecialCharactersImages;

        private TextMesh TextMesh;
        private DiscussionTextWindowDimensions discussionTextWindowDimensions;
        private DiscussionTextPlayerEngine DiscussionTextPlayerEngine;

        public DiscussionText(string initialRawText, DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent,
            TextOnlyDiscussionWindowDimensionsComponent TextOnlyDiscussionWindowDimensionsComponent, DiscussionHeightChangeListener DiscussionHeightChangeListener, Text textAreaText)
        {

            this.initialRawText = initialRawText;
            this.transformedInitialRawText = Regex.Unescape(this.initialRawText);

            #region Special Character Image mapping
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
            #endregion

            this.discussionTextWindowDimensions = new DiscussionTextWindowDimensions(DiscussionWindowDimensionsComponent, TextOnlyDiscussionWindowDimensionsComponent);
            this.DiscussionTextPlayerEngine = new DiscussionTextPlayerEngine(TextOnlyDiscussionWindowDimensionsComponent, this.discussionTextWindowDimensions, DiscussionHeightChangeListener);

            this.TextMesh = new TextMesh(textAreaText, DiscussionWindowDimensionsComponent);
        }

        #region Data Retrieval
        public List<string> LinedTruncatedText { get => linedTruncatedText; }
        public string OverlappedText { get => overlappedText; }

        public float GetFinalWidth() { return this.discussionTextWindowDimensions.FinalWidth; }
        public float GetWindowHeight(int lineNb) { return this.discussionTextWindowDimensions.GetWindowHeight(lineNb, this.TextMesh); }
        public int GetDisplayedLineNb() { return this.DiscussionTextPlayerEngine.DisplayedLineNb; }
        #endregion

        #region Writing
        public void Increment()
        {
            this.DiscussionTextPlayerEngine.Increment(this.TextMesh, this.SpecialCharactersImages);
        }
        #endregion

        #region Logical Conditions
        public bool IsAllowedToIncrementEngine()
        {
            return this.DiscussionTextPlayerEngine.IsAllowedToIncrementEngine();
        }
        #endregion

        public void ComputeTruncatedText(RectTransform discussionWindowTransform)
        {
            var initialSizeDelta = discussionWindowTransform.sizeDelta;

            var generatedText = this.TextMesh.ForceRefresh(this.transformedInitialRawText, new Vector2(this.discussionTextWindowDimensions.GetMaxWindowWidth(), this.discussionTextWindowDimensions.GetMaxWindowHeight(this.TextMesh)));

            var truncatedText = this.transformedInitialRawText.Substring(0, generatedText.characterCountVisible);
            this.overlappedText = this.transformedInitialRawText.Substring(generatedText.characterCountVisible, this.transformedInitialRawText.Length - generatedText.characterCountVisible);

            truncatedText = truncatedText.Trim(this.TrimmedCharForSanitaze);

            this.TextMesh.ForceRefresh(truncatedText);

            this.discussionTextWindowDimensions.ComputeFinalDimensions();

            generatedText = this.TextMesh.ForceRefresh(truncatedText, new Vector2(this.GetFinalWidth(), this.discussionTextWindowDimensions.GetMaxWindowHeight(this.TextMesh)));

            this.linedTruncatedText = new List<string>();
            for (int i = 0; i < generatedText.lines.Count; i++)
            {
                int startIndex = generatedText.lines[i].startCharIdx;
                int endIndex = (i == generatedText.lines.Count - 1) ? truncatedText.Length
                    : generatedText.lines[i + 1].startCharIdx;
                int length = endIndex - startIndex;

                string lineToAdd = truncatedText.Substring(startIndex, length).Trim(this.TrimmedCharForSanitaze);
                this.linedTruncatedText.Add(lineToAdd);
            }

            this.TextMesh.ForceRefresh("");

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

        public void ComputeFinalDimensions()
        {
            this.finalWidth = DiscussionWindowDimensionsComponent.MaxWindowWidth;
        }

        public float GetMaxWindowHeight(TextMesh TextMesh)
        {
            return this.GetWindowHeight(this.TextOnlyDiscussionWindowDimensionsComponent.MaxLineDisplayed, TextMesh);
        }

        public float GetWindowHeight(int lineNb, TextMesh TextMesh)
        {
            return Mathf.Max(DiscussionWindowDimensionsComponent.MinWindowHeight, ((TextMesh.TextGenerationSettings.font.lineHeight + TextMesh.TextGenerationSettings.lineSpacing) * lineNb) + (DiscussionWindowDimensionsComponent.MarginDown + DiscussionWindowDimensionsComponent.MarginUp));
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
        private string currentDisplayedTextUnModified;
        private int displayedLineNb;

        private Regex textSpecialImageRegex;

        public int DisplayedLineNb { get => displayedLineNb; }

        public void StartWriting(DiscussionText discussionText)
        {
            this.targetText = String.Join("\n", discussionText.LinedTruncatedText.ToArray());
            this.currentDisplayedTextUnModified = String.Empty;
            this.displayedLineNb = 1;
            this.textSpecialImageRegex = new Regex(DiscussionText.SpecialCharacterImageTemplate);
        }

        public void Increment(TextMesh TextMesh, List<Image> SpecialCharactersImages)
        {
            if (currentDisplayedTextUnModified.Length < targetText.Length)
            {
                var charToAdd = targetText[currentDisplayedTextUnModified.Length];

                if (charToAdd == '#')
                {
                    currentDisplayedTextUnModified += charToAdd;
                    currentDisplayedTextUnModified += targetText[currentDisplayedTextUnModified.Length];
                    currentDisplayedTextUnModified += targetText[currentDisplayedTextUnModified.Length];
                }
                else
                {
                    currentDisplayedTextUnModified += charToAdd;
                }

                TextMesh.ForceRefresh(currentDisplayedTextUnModified);

                int specialImageMatchCount = 0;
                foreach (var imageVertices in TextMesh.FindOccurences(DiscussionText.SpecialCharacterImageTemplate))
                {
                    Vector2 imagePosition = imageVertices.Center();
                    SpecialCharactersImages[specialImageMatchCount].gameObject.SetActive(true);
                    SpecialCharactersImages[specialImageMatchCount].transform.localPosition = imagePosition;
                    ((RectTransform)(SpecialCharactersImages[specialImageMatchCount].transform)).sizeDelta = new Vector2(imageVertices.Width(), imageVertices.Width());
                    specialImageMatchCount += 1;
                }

                var newDisplayedLineNb = Mathf.Min(this.currentDisplayedTextUnModified.Split('\n').Length, this.TextOnlyDiscussionWindowDimensionsComponent.MaxLineDisplayed);
                if (newDisplayedLineNb != this.displayedLineNb)
                {
                    this.DiscussionHeightChangeListener.OnHeightChange(this.DiscussionTextWindowDimensions.GetWindowHeight(newDisplayedLineNb, TextMesh));
                }
                this.displayedLineNb = newDisplayedLineNb;
            }
        }

        public bool IsAllowedToIncrementEngine()
        {
            return currentDisplayedTextUnModified.Length != targetText.Length;
        }

    }

    public interface DiscussionHeightChangeListener
    {
        void OnHeightChange(float newHeight);
    }

    class TextMesh
    {
        private DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent;

        private CanvasRenderer canvasRenderer;
        private TextGenerationSettings textGenerationSettings;
        private TextGenerator textGenerator;
        private string lastTextUsedForGeneration;
        private Mesh Mesh;

        public TextMesh(Text text, DiscussionWindowDimensionsComponent DiscussionWindowDimensionsComponent)
        {
            this.DiscussionWindowDimensionsComponent = DiscussionWindowDimensionsComponent;

            this.canvasRenderer = text.GetComponent<CanvasRenderer>();
            this.textGenerationSettings = text.GetGenerationSettings(Vector2.zero);
            this.textGenerator = new TextGenerator();
            this.textGenerator.Invalidate();
            text.enabled = false;
        }

        public TextGenerator ForceRefresh(string text, Nullable<Vector2> unmargedExtends = null)
        {
            this.textGenerator.Invalidate();
            if (unmargedExtends.HasValue)
            {
                this.textGenerationSettings.generationExtents = unmargedExtends.Value + new Vector2(-DiscussionWindowDimensionsComponent.MarginLeft - DiscussionWindowDimensionsComponent.MarginRight,
                                               -DiscussionWindowDimensionsComponent.MarginUp - DiscussionWindowDimensionsComponent.MarginDown);
            }
            this.textGenerator.Populate(text, this.TextGenerationSettings);

            this.TextGenToMesh(this.textGenerator, out Mesh textMesh);
            this.canvasRenderer.SetMesh(textMesh);
            this.canvasRenderer.SetMaterial(this.textGenerationSettings.font.material, null);
            this.Mesh = textMesh;
            this.lastTextUsedForGeneration = text;

            return this.textGenerator;
        }

        public List<LetterVertices> FindOccurences(string patternToFind)
        {
            var sanitizedLastTextUsedForGeneration = this.lastTextUsedForGeneration.Replace(" ", "").Replace("\n", "");
            List<LetterVertices> foundVertices = new List<LetterVertices>();
            foreach (Match match in new Regex(patternToFind).Matches(sanitizedLastTextUsedForGeneration))
            {
                if (match.Success)
                {
                    foundVertices.Add(new LetterVertices()
                    {
                        TopLeft = this.Mesh.vertices[match.Index * 4],
                        TopRight = this.Mesh.vertices[((match.Index + match.Length - 1) * 4) + 1],
                        BottomRight = this.Mesh.vertices[((match.Index + match.Length - 1) * 4) + 2],
                        BottomLeft = this.Mesh.vertices[(match.Index * 4) + 3]
                    });
                }
            }
            return foundVertices;
        }

        private void TextGenToMesh(TextGenerator generator, out Mesh mesh)
        {
            mesh = new Mesh();

            mesh.vertices = generator.verts.Select(v => v.position).ToArray();
            mesh.colors32 = generator.verts.Select(v => v.color).ToArray();
            mesh.uv = generator.verts.Select(v => v.uv0).ToArray();
            var triangles = new int[generator.vertexCount * 6];
            for (var i = 0; i < mesh.vertices.Length / 4; i++)
            {
                var startVerticeIndex = i * 4;
                var startTriangleIndex = i * 6;
                triangles[startTriangleIndex++] = startVerticeIndex;
                triangles[startTriangleIndex++] = startVerticeIndex + 1;
                triangles[startTriangleIndex++] = startVerticeIndex + 2;
                triangles[startTriangleIndex++] = startVerticeIndex;
                triangles[startTriangleIndex++] = startVerticeIndex + 2;
                triangles[startTriangleIndex] = startVerticeIndex + 3;
            }
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
        }

        public Transform Transform
        {
            get
            {
                return this.canvasRenderer.transform;
            }
        }

        public TextGenerationSettings TextGenerationSettings { get => textGenerationSettings; }
    }

    class LetterVertices
    {
        public Vector3 TopLeft;
        public Vector3 TopRight;
        public Vector3 BottomRight;
        public Vector3 BottomLeft;

        public Vector2 Center()
        {
            return (this.TopLeft + this.TopRight + this.BottomLeft + this.BottomRight) / 4f;
        }

        public float Width()
        {
            return Mathf.Abs(this.TopRight.x - this.TopLeft.x);
        }
    }
}
