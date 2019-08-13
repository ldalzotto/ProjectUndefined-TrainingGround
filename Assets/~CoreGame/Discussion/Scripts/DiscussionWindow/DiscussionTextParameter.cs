using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{
    public class DiscussionTextParameter
    {
        public static Dictionary<ParameterType, string> ParameterAlias = new Dictionary<ParameterType, string>()
        {
            {ParameterType.INPUT_PARAMETER, "InputParam" }
        };

        public const string TransformedParameterTemplate = "#0";

        private Regex BaseParameterRegex = new Regex("\\${.*?}");

        private List<IParameterDisplayContent> parameterDisplayContent;

        public List<IParameterDisplayContent> ParameterDisplayContent { get => parameterDisplayContent; }

        public string ParseParameters(string inputText)
        {
            this.parameterDisplayContent = new List<IParameterDisplayContent>();

            Match parameterMatch = null;
            do
            {
                parameterMatch = this.BaseParameterRegex.Match(inputText);
                if (parameterMatch.Success)
                {
                    if (parameterMatch.Value.Contains(ParameterAlias[ParameterType.INPUT_PARAMETER]))
                    {
                        var instaciatedImage = MonoBehaviour.Instantiate(PrefabContainer.Instance.InputBaseImage);
                        instaciatedImage.gameObject.SetActive(false);

                        inputText = inputText.Substring(0, parameterMatch.Index)
                                      + TransformedParameterTemplate
                                      + inputText.Substring(parameterMatch.Index + parameterMatch.Value.Length, inputText.Length - (parameterMatch.Index + parameterMatch.Value.Length));
                        this.parameterDisplayContent.Add(new InputParameterDisplayContent("D", instaciatedImage));

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
        public string InputText;
        public Image IconImage;

        public InputParameterDisplayContent(string inputText, Image iconImage)
        {
            InputText = inputText;
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
