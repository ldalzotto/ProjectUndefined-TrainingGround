using UnityEngine;
using UnityEngine.UI;

namespace CoreGame
{
    public class InputImageType : MonoBehaviour
    {
        public static InputImageType Instantiate()
        {
            InputImageType InputImageType = MonoBehaviour.Instantiate(PrefabContainer.Instance.InputBaseImage);
            InputImageType.Init();
            return InputImageType;
        }

        #region Internal Dependencies
        private Text KeyText; 
        #endregion

        public void Init()
        {
            this.KeyText = GetComponentInChildren<Text>();
        }
        
        public void SetKey(string key)
        {
            this.KeyText.text = key;
        }
    }
}
