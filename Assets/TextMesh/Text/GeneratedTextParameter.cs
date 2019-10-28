namespace CoreGame
{
    /*
    public class GeneratedTextParameter
    {
        #region Constants

        public static Dictionary<ParameterType, string> ParameterAlias = new Dictionary<ParameterType, string>()
        {
            {ParameterType.INPUT_PARAMETER, "InputParam"}
        };

        public const string TransformedParameterImageTemplate = "#0";

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

        public List<IParameterDisplayContent> ParameterDisplayContent
        {
            get => parameterDisplayContent;
        }

        #endregion

        public GeneratedTextParameter(ReadOnlyCollection<InputParameter> InputParameters, InputConfiguration inputConfiguration)
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
                        //We replace the parameter text by a template that will be hidden by image
                        var parameterOrderNumberMatch = ParameterNumberExtractorRegex.Match(parameterMatch.Value);
                        if (parameterOrderNumberMatch.Success)
                        {
                            var inputConfigurationData = this.InputConfiguration.ConfigurationInherentData[this.InputParameters[Convert.ToInt32(parameterOrderNumberMatch.Value)].inputID];

                            InputImageType instaciatedImage = InputImageType.Instantiate(inputConfigurationData);

                            if (instaciatedImage != null)
                            {
                                instaciatedImage.gameObject.SetActive(false);

                                inputText = inputText.Substring(0, parameterMatch.Index)
                                            + TransformedParameterImageTemplate
                                            + inputText.Substring(parameterMatch.Index + parameterMatch.Value.Length, inputText.Length - (parameterMatch.Index + parameterMatch.Value.Length));

                                this.parameterDisplayContent.Add(new InputParameterDisplayContent(instaciatedImage));
                            }
                        }
                    }
                    else
                    {
                        //We remove the parameter text
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
            foreach (var imageVertices in textMesh.FindOccurences(TransformedParameterImageTemplate))
            {
                if (this.parameterDisplayContent[transformedParameterCount].GetType() == typeof(InputParameterDisplayContent))
                {
                    var InputParameterDisplayContent = (InputParameterDisplayContent) this.parameterDisplayContent[transformedParameterCount];
                    Vector2 imagePosition = imageVertices.Center();
                    InputParameterDisplayContent.ImageGameObject.SetActive(true);
                    InputParameterDisplayContent.ImageGameObject.transform.SetParent(textMesh.CanvasRenderer.transform);
                    InputParameterDisplayContent.ImageGameObject.transform.localPosition = imagePosition;
                    ((RectTransform) (InputParameterDisplayContent.ImageGameObject.transform)).sizeDelta = Vector2.one * imageVertices.Width();
                    InputParameterDisplayContent.ImageGameObject.transform.localScale = Vector3.one;
                    transformedParameterCount += 1;

                    if (InputParameterDisplayContent.GetInputImageTypeInstanceType() == InputImageTypeInstanceType.KEY)
                    {
                        InputParameterDisplayContent.IconImage.SetTextFontSize((int) Math.Floor(imageVertices.Width() * 0.75f));
                    }
                }
            }

            return transformedParameterCount;
        }

        public string GetFullTransformedParameterTemplate(char firstChar)
        {
            if (firstChar == TransformedParameterImageTemplate[0])
            {
                return TransformedParameterImageTemplate;
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
        public GameObject ImageGameObject;

        public InputParameterDisplayContent(GameObject imageGameObject)
        {
            this.ImageGameObject = imageGameObject;
        }

        public InputImageTypeInstanceType GetInputImageTypeInstanceType()
        {
            return this.IconImage.InputImageTypeInstanceType;
        }

        public void OnDiscussionContinue()
        {
            if (this.ImageGameObject.activeSelf)
            {
                this.ImageGameObject.SetActive(false);
            }
        }

        public void OnDiscussionTerminated()
        {
            MonoBehaviour.Destroy(this.ImageGameObject);
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

        public int TransformedParameterCurrentCountOffset
        {
            get => transformedParameterCurrentCountOffset;
        }

        #endregion

        #region External Events

        public void OnDiscussionEngineIncremented(in TextPlayerEngine DiscussionTextPlayerEngine, int parameterNbInThisIncrement)
        {
            if (!DiscussionTextPlayerEngine.IsAllowedToIncrementEngine())
            {
                this.transformedParameterCurrentCountOffset += parameterNbInThisIncrement;
            }
        }

        #endregion
    }
    */
}