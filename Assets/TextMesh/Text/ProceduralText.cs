﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{
    public class ProceduralText
    {
        private char[] TrimmedCharForSanitaze = new char[] {' ', '\n'};

        #region State

        private string initialRawText;
        private string transformedInitialRawText;
        private string overlappedText;
        private List<string> linedTruncatedText;

        #endregion

        #region Internal Managers

        #region Parameters management

        //  private GeneratedTextParameter DiscussionTextParameter;

        #endregion

        private TextMesh TextMesh;
        private GeneratedTextDimensions textDimensions;
        private TextPlayerEngine TextPlayerEngine;

        #endregion

        public ProceduralText(string initialRawText /*,  GeneratedTextParameter GeneratedTextParameter*/, GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent, Text textAreaText)
        {
            this.initialRawText = initialRawText;
            this.transformedInitialRawText = Regex.Unescape(this.initialRawText);

            #region Special Character Image mapping

            //  this.DiscussionTextParameter = GeneratedTextParameter;
            //   this.transformedInitialRawText = this.DiscussionTextParameter.ParseParameters(this.transformedInitialRawText);

            #endregion

            this.textDimensions = new GeneratedTextDimensions(GeneratedTextDimensionsComponent);
            this.TextPlayerEngine = new TextPlayerEngine(GeneratedTextDimensionsComponent, this.textDimensions);

            this.TextMesh = new TextMesh(textAreaText);
        }

        #region Data Retrieval

        public List<string> LinedTruncatedText
        {
            get => linedTruncatedText;
        }

        public string OverlappedText
        {
            get => overlappedText;
        }

        public float GetWindowHeight(int lineNb)
        {
            return this.textDimensions.GetWindowHeight(this.TextMesh);
        }

        public float GetWindowHeight()
        {
            return this.textDimensions.GetWindowHeight(this.TextMesh);
        }

        public float GetWindowWidth()
        {
            return this.textDimensions.GetWindowWidth(this.TextMesh);
        }

        #endregion

        #region Writing

        public void Increment()
        {
            this.TextPlayerEngine.Increment(this.TextMesh /*, this.DiscussionTextParameter*/);
        }

        #endregion

        #region Logical Conditions

        public bool IsDisplayEngineFinished()
        {
            return this.TextPlayerEngine.IsDisplayEngineFinished();
        }

        #endregion

        #region External Events

        public void MoveToNextPage()
        {
            //   this.DiscussionTextParameter.OnDiscussionContinue();
            this.transformedInitialRawText = this.overlappedText;
            this.overlappedText = string.Empty;
            this.linedTruncatedText.Clear();
            this.TextMesh.Clear();
            this.CalculateCurrentPage();
        }

        public void OnDiscussionTerminated()
        {
            // this.DiscussionTextParameter.OnDiscussionTerminated();
        }

        #endregion

        #region Entry points

        public void CalculateCurrentPage()
        {
            var generatedText = this.TextMesh.ForceRefreshInternalGeneration(this.transformedInitialRawText, new Vector2(this.textDimensions.GetMaxWindowWidth(), this.textDimensions.GetMaxWindowHeight(this.TextMesh)));

            var truncatedText = this.transformedInitialRawText.Substring(0, generatedText.characterCountVisible);
            this.overlappedText = this.transformedInitialRawText.Substring(generatedText.characterCountVisible, this.transformedInitialRawText.Length - generatedText.characterCountVisible);

            truncatedText = truncatedText.Trim(this.TrimmedCharForSanitaze);

            generatedText = this.TextMesh.ForceRefreshInternalGeneration(truncatedText, new Vector2(this.textDimensions.GetMaxWindowWidth(), this.textDimensions.GetMaxWindowHeight(this.TextMesh)));

            this.linedTruncatedText = new List<string>();
            for (int i = 0; i < generatedText.lines.Count; i++)
            {
                int startIndex = generatedText.lines[i].startCharIdx;
                int endIndex = (i == generatedText.lines.Count - 1)
                    ? truncatedText.Length
                    : generatedText.lines[i + 1].startCharIdx;
                int length = endIndex - startIndex;

                string lineToAdd = truncatedText.Substring(startIndex, length).Trim(this.TrimmedCharForSanitaze);
                this.linedTruncatedText.Add(lineToAdd);
            }

            this.TextMesh.GenerateFinalMeshFromTextGenerator();
            this.TextPlayerEngine.StartWriting(this);
        }

        public void GenerateAndDisplayAllText()
        {
            this.CalculateCurrentPage();
            this.TextPlayerEngine.RenderEverything(this.TextMesh /*, this.DiscussionTextParameter*/);
        }

        #endregion
    }

    public class TextPlayerEngine
    {
        #region Trackers

        //   private TransformedParameterCounterTracker TransformedParameterCounterTracker;

        #endregion

        private GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent;
        private GeneratedTextDimensions GeneratedTextDimensions;

        public TextPlayerEngine(GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent,
            GeneratedTextDimensions GeneratedTextDimensions)
        {
            this.GeneratedTextDimensionsComponent = GeneratedTextDimensionsComponent;
            this.GeneratedTextDimensions = GeneratedTextDimensions;
            // this.TransformedParameterCounterTracker = new TransformedParameterCounterTracker();
        }

        private string targetText;
        private string currentDisplayedTextUnModified;


        public void StartWriting(ProceduralText discussionText)
        {
            this.targetText = String.Join("\n", discussionText.LinedTruncatedText.ToArray());
            this.currentDisplayedTextUnModified = String.Empty;
        }

        public void Increment(TextMesh TextMesh /*, GeneratedTextParameter DiscussionTextParameter*/)
        {
            var parameterNbInThisIncrement = 0;
            if (currentDisplayedTextUnModified.Length < targetText.Length)
            {
                var stringToAdd = targetText[currentDisplayedTextUnModified.Length].ToString();
                // var stringToAdd = DiscussionTextParameter.GetFullTransformedParameterTemplate(targetText[currentDisplayedTextUnModified.Length]);

                for (var i = 0; i < stringToAdd.Length; i++)
                {
                    TextMesh.IncrementChar(stringToAdd[i]);
                    currentDisplayedTextUnModified += stringToAdd[i];
                }

                //   parameterNbInThisIncrement = Math.Max(parameterNbInThisIncrement, DiscussionTextParameter.ProcessParametersOnFinalTextMesh(TextMesh, this.TransformedParameterCounterTracker));

                //       this.TransformedParameterCounterTracker.OnDiscussionEngineIncremented(this, parameterNbInThisIncrement);
            }
        }

        public void RenderEverything(TextMesh TextMesh /*, GeneratedTextParameter DiscussionTextParameter*/)
        {
            while (currentDisplayedTextUnModified.Length != targetText.Length)
            {
                this.Increment(TextMesh /*, DiscussionTextParameter*/);
            }
        }

        public bool IsDisplayEngineFinished()
        {
            return currentDisplayedTextUnModified.Length != targetText.Length;
        }
    }

    public class TextMesh
    {
        private CanvasRenderer canvasRenderer;
        private TextGenerationSettings textGenerationSettings;
        private TextGenerator textGenerator;

        private Color textColor;
        private string lastTextUsedToSet;
        private string lastMeshedText;

        private Mesh mesh;
        private MeshDimensions MeshDimensions;

        public TextMesh(Text text)
        {
            this.canvasRenderer = text.canvasRenderer;
            this.textGenerationSettings = text.GetGenerationSettings(Vector2.zero);
            this.textGenerator = new TextGenerator();
            this.textGenerator.Invalidate();
            this.mesh = new Mesh();
            this.MeshDimensions = new MeshDimensions();
            this.textColor = text.color;

            text.enabled = false;
            this.canvasRenderer.SetMaterial(text.font.material, null);
        }

        public TextGenerator ForceRefreshInternalGeneration(string text, Nullable<Vector2> unmargedExtends = null)
        {
            this.textGenerator.Invalidate();
            if (unmargedExtends.HasValue)
            {
                this.textGenerationSettings.generationExtents = unmargedExtends.Value;
            }

            this.textGenerator.Populate(text, this.TextGenerationSettings);
            return this.textGenerator;
        }

        public void GenerateFinalMeshFromTextGenerator()
        {
            this.TextGenToMesh(this.textGenerator, ref this.mesh);
            this.lastTextUsedToSet = string.Empty;
            this.lastMeshedText = string.Empty;
            this.canvasRenderer.SetMesh(this.mesh);
        }

        public void IncrementChar(char characterAdded)
        {
            this.lastTextUsedToSet += characterAdded;
            if (characterAdded != ' ' && characterAdded != '\n')
            {
                this.lastMeshedText += characterAdded;
                var newColors = this.mesh.colors;
                newColors[((this.lastMeshedText.Length - 1) * 4)] = this.textColor;
                newColors[((this.lastMeshedText.Length - 1) * 4) + 1] = this.textColor;
                newColors[((this.lastMeshedText.Length - 1) * 4) + 2] = this.textColor;
                newColors[((this.lastMeshedText.Length - 1) * 4) + 3] = this.textColor;

                this.MeshDimensions.OnLetterVerticesShowed(this.mesh.vertices[((this.lastMeshedText.Length - 1) * 4)]);
                this.MeshDimensions.OnLetterVerticesShowed(this.mesh.vertices[((this.lastMeshedText.Length - 1) * 4) + 1]);
                this.MeshDimensions.OnLetterVerticesShowed(this.mesh.vertices[((this.lastMeshedText.Length - 1) * 4) + 2]);
                this.MeshDimensions.OnLetterVerticesShowed(this.mesh.vertices[((this.lastMeshedText.Length - 1) * 4) + 3]);

                this.mesh.colors = newColors;

                this.canvasRenderer.SetMesh(this.mesh);
            }
        }

        public List<LetterVertices> FindOccurences(string patternToFind)
        {
            var sanitizedLastTextUsedForGeneration = this.lastTextUsedToSet.Replace(" ", "").Replace("\n", "");
            List<LetterVertices> foundVertices = new List<LetterVertices>();
            var meshVertices = this.mesh.vertices;
            foreach (Match match in new Regex(patternToFind).Matches(sanitizedLastTextUsedForGeneration))
            {
                if (match.Success)
                {
                    foundVertices.Add(new LetterVertices()
                    {
                        TopLeft = meshVertices[match.Index * 4],
                        TopRight = meshVertices[((match.Index + match.Length - 1) * 4) + 1],
                        BottomRight = meshVertices[((match.Index + match.Length - 1) * 4) + 2],
                        BottomLeft = meshVertices[(match.Index * 4) + 3]
                    });
                }
            }

            return foundVertices;
        }

        private void TextGenToMesh(TextGenerator generator, ref Mesh mesh)
        {
            var scaleMatrix = Matrix4x4.Scale(new Vector3(this.textGenerationSettings.scaleFactor, this.textGenerationSettings.scaleFactor, this.textGenerationSettings.scaleFactor)).inverse;
            mesh.vertices = generator.verts.Select(v => scaleMatrix.MultiplyPoint(v.position)).ToArray();
            mesh.colors32 = new Color32[mesh.vertexCount];
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
        }

        public Transform Transform
        {
            get { return this.canvasRenderer.transform; }
        }

        public TextGenerationSettings TextGenerationSettings
        {
            get => textGenerationSettings;
        }

        public CanvasRenderer CanvasRenderer
        {
            get => canvasRenderer;
        }

        public float GetMeshWidth()
        {
            return this.MeshDimensions.Width;
        }

        public float GetMeshHeight()
        {
            return this.MeshDimensions.Height;
        }

        public void Clear()
        {
            this.mesh.Clear();
            this.MeshDimensions.Clear();
        }
    }

    public class LetterVertices
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

    class MeshDimensions
    {
        private float MinY;
        private float MaxY;

        private float MinX;
        private float MaxX;

        public float Height { get; private set; }
        public float Width { get; private set; }

        public void OnLetterVerticesShowed(Vector2 verticePosition)
        {
            this.MinX = Mathf.Min(this.MinX, verticePosition.x);
            this.MaxX = Mathf.Max(this.MaxX, verticePosition.x);

            this.Width = Mathf.Abs(MaxX - MinX);

            this.MinY = Mathf.Min(this.MinY, verticePosition.y);
            this.MaxY = Mathf.Max(this.MaxY, verticePosition.y);

            this.Height = Mathf.Abs(MaxY - MinY);
        }

        public void Clear()
        {
            this.Width = 0f;
            this.MinX = 0f;
            this.MaxX = 0f;
            this.Height = 0f;
            this.MinY = 0f;
            this.MaxY = 0f;
        }
    }
}