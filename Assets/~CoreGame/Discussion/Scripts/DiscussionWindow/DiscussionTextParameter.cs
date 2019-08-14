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
        #endregion

        #region State
        private List<IParameterDisplayContent> parameterDisplayContent;
        private ReadOnlyCollection<InputParameter> InputParameters;
        #endregion

        #region Data Retrieval
        public List<IParameterDisplayContent> ParameterDisplayContent { get => parameterDisplayContent; }
        #endregion

        public DiscussionTextParameter(ReadOnlyCollection<InputParameter> InputParameters, InputConfiguration inputConfiguration)
        {
            this.InputParameters = InputParameters;
            this.InputConfiguration = inputConfiguration;
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
                            var instaciatedImage = InputImageType.Instantiate();
                            instaciatedImage.gameObject.SetActive(false);

                            inputText = inputText.Substring(0, parameterMatch.Index)
                                          + TransformedParameterTemplate
                                          + inputText.Substring(parameterMatch.Index + parameterMatch.Value.Length, inputText.Length - (parameterMatch.Index + parameterMatch.Value.Length));

                            var inputConfigurationData = this.InputConfiguration.ConfigurationInherentData[this.InputParameters[Convert.ToInt32(parameterOrderNumberMatch.Value)].inputID];
                            this.parameterDisplayContent.Add(new InputParameterDisplayContent(inputConfigurationData, instaciatedImage));
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

        public void ProcessParametersOnFinalTextMesh(TextMesh textMesh)
        {
            int transformedParameterMatchCount = 0;
            foreach (var imageVertices in textMesh.FindOccurences(TransformedParameterTemplate))
            {
                if (this.parameterDisplayContent[transformedParameterMatchCount].GetType() == typeof(InputParameterDisplayContent))
                {
                    var InputParameterDisplayContent = (InputParameterDisplayContent)this.parameterDisplayContent[transformedParameterMatchCount];
                    Vector2 imagePosition = imageVertices.Center();
                    InputParameterDisplayContent.IconImage.gameObject.SetActive(true);
                    InputParameterDisplayContent.IconImage.transform.SetParent(textMesh.CanvasRenderer.transform);
                    InputParameterDisplayContent.IconImage.transform.localPosition = imagePosition;
                    ((RectTransform)(InputParameterDisplayContent.IconImage.transform)).sizeDelta = new Vector2(imageVertices.Width(), imageVertices.Width());
                    InputParameterDisplayContent.IconImage.SetKey(InputParameterDisplayContent.InputConfigurationInherentData.AttributedKeys[0].ToString()[0].ToString());
                    transformedParameterMatchCount += 1;
                }
            }
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
        public void OnTextDestroy()
        {
            if (this.parameterDisplayContent != null)
            {
                this.parameterDisplayContent.ForEach((parameter) => parameter.OnTextDestroy());
            }
        }
        #endregion
    }

    public interface IParameterDisplayContent
    {
        void OnTextDestroy();
    }

    public class InputParameterDisplayContent : IParameterDisplayContent
    {
        public InputConfigurationInherentData InputConfigurationInherentData;
        public InputImageType IconImage;

        public InputParameterDisplayContent(InputConfigurationInherentData inputConfigurationInherentData, InputImageType iconImage)
        {
            InputConfigurationInherentData = inputConfigurationInherentData;
            IconImage = iconImage;
        }

        public void OnTextDestroy()
        {
            MonoBehaviour.Destroy(this.IconImage.gameObject);
        }
    }

    public enum ParameterType
    {
        INPUT_PARAMETER
    }
}
