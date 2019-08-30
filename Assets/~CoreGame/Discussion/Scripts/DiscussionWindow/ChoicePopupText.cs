using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{

    public class ChoicePopupText : MonoBehaviour
    {
        private DiscussionChoice discussionChoice;
        private GeneratedText generatedText;

        public DiscussionChoice DiscussionChoice { get => discussionChoice; }

        public void Init(DiscussionChoice choice, DiscussionTextConfiguration DiscussionTextConfiguration, GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent)
        {
            var text = GetComponent<Text>();
            discussionChoice = choice;
            var DiscussionTextInherentData = DiscussionTextConfiguration.ConfigurationInherentData[choice.Text];
            this.generatedText = new GeneratedText(DiscussionTextInherentData.Text, new GeneratedTextParameter(DiscussionTextInherentData.InputParameters.AsReadOnly(), null),
                  GeneratedTextDimensionsComponent, null, text);
            this.generatedText.GenerateAndDisplayAllText();
        }

        public float GetTextCharacterLength()
        {
            return this.generatedText.GetWindowWidth();
        }

        public float GetTextCharacterHeight()
        {
            return this.generatedText.GetWindowHeight(this.generatedText.GetDisplayedLineNb());
        }

    }

}