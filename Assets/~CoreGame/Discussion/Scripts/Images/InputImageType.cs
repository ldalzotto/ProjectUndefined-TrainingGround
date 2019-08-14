using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{
    public class InputImageType : MonoBehaviour
    {
        private InputImageTypeInstanceType InputImageTypeInstanceType;

        public static InputImageType Instantiate(InputImageTypeInstanceType InputImageTypeInstanceType)
        {
            InputImageType InputImageType = null;
            if (InputImageTypeInstanceType == InputImageTypeInstanceType.KEY)
            {
                InputImageType = MonoBehaviour.Instantiate(PrefabContainer.Instance.InputBaseImage);
            }
            else if (InputImageTypeInstanceType == InputImageTypeInstanceType.LEFT_MOUSE)
            {
                InputImageType = MonoBehaviour.Instantiate(PrefabContainer.Instance.LeftMouseBaseImage);
            }
            else if (InputImageTypeInstanceType == InputImageTypeInstanceType.RIGHT_MOUSE)
            {
                InputImageType = MonoBehaviour.Instantiate(PrefabContainer.Instance.RightMouseBaseImage);
            }

            InputImageType.Init(InputImageTypeInstanceType);
            return InputImageType;
        }

        #region Internal Dependencies
        private Text KeyText;
        #endregion

        public void Init(InputImageTypeInstanceType InputImageTypeInstanceType)
        {
            this.InputImageTypeInstanceType = InputImageTypeInstanceType;
            this.KeyText = GetComponentInChildren<Text>();
        }

        public void SetKey(string key)
        {
            this.KeyText.text = key;
        }

        public void SetTextFontSize(int fontSize)
        {
            this.KeyText.fontSize = fontSize;
        }
    }

    public enum InputImageTypeInstanceType
    {
        NONE = 0,
        KEY = 1,
        LEFT_MOUSE = 2,
        RIGHT_MOUSE = 3
    }
}
