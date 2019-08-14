﻿using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{

    public class ChoicePopupText : MonoBehaviour
    {
        private Text text;
        private DiscussionChoice discussionChoice;

        private TextGenerationSettings TextGenerationSettings;

        public DiscussionChoice DiscussionChoice { get => discussionChoice; }

        private void Awake()
        {
            text = GetComponent<Text>();
        }

        public void SetDiscussionChoice(DiscussionChoice choice, DiscussionTextConfiguration DiscussionTextConfiguration)
        {
            discussionChoice = choice;
            this.text.text = DiscussionTextConfiguration.ConfigurationInherentData[choice.Text].Text;
            TextGenerationSettings = new TextGenerationSettings();
            TextGenerationSettings.font = text.font;
            TextGenerationSettings.fontSize = text.fontSize;
            TextGenerationSettings.fontStyle = text.fontStyle;
        }

        public int GetFontSize()
        {
            return text.fontSize;
        }

        public float GetTextCharacterLength()
        {
            return text.cachedTextGenerator.GetPreferredWidth(text.text, TextGenerationSettings);
        }

        public float GetTextCharacterHeight()
        {
            return text.cachedTextGenerator.GetPreferredHeight(text.text, TextGenerationSettings);
        }

    }

}