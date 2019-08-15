using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CoreGame
{
    public class DiscussionTextParameter
    {
        #region Constants
        public static Dictionary<ParameterType, string> ParameterAlias = new Dictionary<ParameterType, string>()
        {
            {ParameterType.INPUT_PARAMETER, "InputParam" }
        };
        public const string TransformedParameterTemplate = "#0";

        private Regex BaseParameterRegex = new Regex("\\${.*?}");
        private Regex ParameterNameExtractorRegex = new Regex("((?![\\${]).*(?=:))");
        private Regex ParameterNumberExtractorRegex = new Regex("((?<=[:]).*(?=}))");
        #endregion

        #region External Dependencies
        private InputConfiguration InputConfiguration;
        private GameInputManager GameInputManager;
        #endregion

        #region State
        private List<IParameterDisplayContent> parameterDisplayContent;
        private ReadOnlyCollection<InputParameter> InputParameters;
        #endregion

        #region Data Retrieval
        public List<IParameterDisplayContent> ParameterDisplayContent { get => parameterDisplayContent; }
        #endregion

        public DiscussionTextParameter(ReadOnlyCollection<InputParameter> InputParameters, InputConfiguration inputConfiguration, GameInputManager gameInputManager)
        {
            this.InputParameters = InputParameters;
            this.InputConfiguration = inputConfiguration;
            this.GameInputManager = gameInputManager;
        }

        public string ParseParameters(string inputText)
        {
            this.parameterDisplayContent = new List<IParameterDisplayContent>();

            Match parameterMatch = null;
            do
            {
                parameterMatch = this.BaseParameterRegex.Match(inputText);
                if (parameterMatch.Success)
                {
                    var ParameterNameMatch = ParameterNameExtractorRegex.Match(parameterMatch.Value);
                    if (ParameterNameMatch.Success && ParameterNameMatch.Value == ParameterAlias[ParameterType.INPUT_PARAMETER])
                    {
                        var parameterOrderNumberMatch = ParameterNumberExtractorRegex.Match(parameterMatch.Value);
                        if (parameterOrderNumberMatch.Success)
                        {
                            var inputConfigurationData = this.InputConfiguration.ConfigurationInherentData[this.InputParameters[Convert.ToInt32(parameterOrderNumberMatch.Value)].inputID];
                            
                            InputImageType instaciatedImage = InputImageType.Instantiate(inputConfigurationData);

                            if (instaciatedImage != null)
                            {
                                instaciatedImage.gameObject.SetActive(false);

                                inputText = inputText.Substring(0, parameterMatch.Index)
                                              + TransformedParameterTemplate
                                              + inputText.Substring(parameterMatch.Index + parameterMatch.Value.Length, inputText.Length - (parameterMatch.Index + parameterMatch.Value.Length));

                                this.parameterDisplayContent.Add(new InputParameterDisplayContent(instaciatedImage));
                            }
                        }
                    }
                    else
                    {
                        inputText = inputText.Substring(0, parameterMatch.Index)
                                      + inputText.Substring(parameterMatch.Index + parameterMatch.Value.Length, inputText.Length - (parameterMatch.Index + parameterMatch.Value.Length));
                    }
                }
            } while (parameterMatch.Success);

            return inputText;
        }

        public int ProcessParametersOnFinalTextMesh(TextMesh textMesh, TransformedParameterCounterTracker TransformedParameterCounterTracker)
        {
            int transformedParameterCount = 0 + TransformedParameterCounterTracker.TransformedParameterCurrentCountOffset;
            foreach (var imageVertices in textMesh.FindOccurences(TransformedParameterTemplate))
            {
                if (this.parameterDisplayContent[transformedParameterCount].GetType() == typeof(InputParameterDisplayContent))
                {
                    var InputParameterDisplayContent = (InputParameterDisplayContent)this.parameterDisplayContent[transformedParameterCount];
                    if (InputParameterDisplayContent.GetInputImageTypeInstanceType() != InputImageTypeInstanceType.NONE)
                    {
                        Vector2 imagePosition = imageVertices.Center();
                        InputParameterDisplayContent.IconImage.gameObject.SetActive(true);
                        InputParameterDisplayContent.IconImage.transform.SetParent(textMesh.CanvasRenderer.transform);
                        InputParameterDisplayContent.IconImage.transform.localPosition = imagePosition;
                        ((RectTransform)(InputParameterDisplayContent.IconImage.transform)).sizeDelta = Vector2.one * imageVertices.Width();
                        InputParameterDisplayContent.IconImage.transform.localScale = Vector3.one;
                        transformedParameterCount += 1;

                        if (InputParameterDisplayContent.GetInputImageTypeInstanceType() == InputImageTypeInstanceType.KEY)
                        {
                            InputParameterDisplayContent.IconImage.SetTextFontSize((int)Math.Floor(imageVertices.Width() * 0.75f));
                        }
                    }
                }
            }
            return transformedParameterCount;
        }

        public string GetFullTransformedParameterTemplate(char firstChar)
        {
            if (firstChar == TransformedParameterTemplate[0])
            {
                return TransformedParameterTemplate;
            }

            return firstChar.ToString();
        }

        #region External Events
        public void OnDiscussionContinue()
        {
            if (this.parameterDisplayContent != null)
            {
                this.parameterDisplayContent.ForEach((parameter) => parameter.OnDiscussionContinue());
            }
        }
        public void OnDiscussionTerminated()
        {
            if (this.parameterDisplayContent != null)
            {
                this.parameterDisplayContent.ForEach((parameter) => parameter.OnDiscussionTerminated());
            }
        }
        #endregion
    }

    public interface IParameterDisplayContent
    {
        void OnDiscussionContinue();
        void OnDiscussionTerminated();
    }

    public class InputParameterDisplayContent : IParameterDisplayContent
    {
        public InputImageType IconImage;

        public InputParameterDisplayContent( InputImageType iconImage)
        {
            IconImage = iconImage;
        }

        public InputImageTypeInstanceType GetInputImageTypeInstanceType()
        {
            return this.IconImage.InputImageTypeInstanceType;
        }

        public void OnDiscussionContinue()
        {
            if (this.IconImage.gameObject.activeSelf)
            {
                this.IconImage.gameObject.SetActive(false);
            }
        }

        public void OnDiscussionTerminated()
        {
            MonoBehaviour.Destroy(this.IconImage.gameObject);
        }
    }

    public enum ParameterType
    {
        INPUT_PARAMETER
    }

    public class TransformedParameterCounterTracker
    {
        private int transformedParameterCurrentCountOffset = 0;

        #region Data Retrieval
        public int TransformedParameterCurrentCountOffset { get => transformedParameterCurrentCountOffset; }
        #endregion

        #region External Events
        public void OnDiscussionEngineIncremented(in DiscussionTextPlayerEngine DiscussionTextPlayerEngine, int parameterNbInThisIncrement)
        {
            if (!DiscussionTextPlayerEngine.IsAllowedToIncrementEngine())
            {
                this.transformedParameterCurrentCountOffset += parameterNbInThisIncrement;
            }
        }
        #endregion
    }
}
