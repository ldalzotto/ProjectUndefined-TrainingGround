using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{
    public class ChoicePopupText : MonoBehaviour
    {
        private DiscussionChoice discussionChoice;
        private ProceduralText _proceduralText;

        public DiscussionChoice DiscussionChoice
        {
            get => discussionChoice;
        }

        public void Init(DiscussionChoice choice, DiscussionTextConfiguration DiscussionTextConfiguration, GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent)
        {
            var text = GetComponent<Text>();
            discussionChoice = choice;
            var DiscussionTextInherentData = DiscussionTextConfiguration.ConfigurationInherentData[choice.Text];
            this._proceduralText = new ProceduralText(DiscussionTextInherentData.Text /*, new GeneratedTextParameter(DiscussionTextInherentData.InputParameters.AsReadOnly(), null)*/,
                GeneratedTextDimensionsComponent, null, text);
            this._proceduralText.GenerateAndDisplayAllText();
        }

        public float GetTextCharacterLength()
        {
            return this._proceduralText.GetWindowWidth();
        }

        public float GetTextCharacterHeight()
        {
            return 0f;
            //   return this._proceduralText.GetWindowHeight(this._proceduralText.GetDisplayedLineNb());
        }
    }
}