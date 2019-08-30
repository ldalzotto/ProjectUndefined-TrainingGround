using UnityEngine;

namespace CoreGame
{
    public class GeneratedTextDimensions
    {
        private GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent;

        public GeneratedTextDimensions(GeneratedTextDimensionsComponent GeneratedTextDimensionsComponent)
        {
            this.GeneratedTextDimensionsComponent = GeneratedTextDimensionsComponent;
        }

        public float GetMaxWindowHeight(TextMesh TextMesh)
        {
            return this.GetWindowHeight(this.GeneratedTextDimensionsComponent.MaxLineDisplayed, TextMesh);
        }

        public float GetWindowHeight(int lineNb, TextMesh TextMesh)
        {
            var scaledLineHeight = TextMesh.TextGenerationSettings.font.lineHeight / TextMesh.TextGenerationSettings.scaleFactor;
            return
                Mathf.Max(GeneratedTextDimensionsComponent.MinWindowHeight, ((scaledLineHeight + TextMesh.TextGenerationSettings.lineSpacing) * lineNb) + (GeneratedTextDimensionsComponent.MarginDown + GeneratedTextDimensionsComponent.MarginUp));
        }

        public float GetMaxWindowWidth()
        {
            return this.GeneratedTextDimensionsComponent.MaxWindowWidth;
        }
    }

    [System.Serializable]
    public class GeneratedTextDimensionsComponent
    {
        public float MarginLeft;
        public float MarginRight;
        public float MarginUp;
        public float MarginDown;
        public float MaxWindowWidth;
        public float MinWindowHeight;
        public int MaxLineDisplayed;
    }

}
