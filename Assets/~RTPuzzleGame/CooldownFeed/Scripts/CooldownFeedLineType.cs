using System;
using UnityEngine;
using UnityEngine.UI;

namespace RTPuzzle
{
    public class CooldownFeedLineType : MonoBehaviour
    {

        #region External Dependencies
        private RTPPlayerAction associatedAction;
        #endregion

        #region Internal Dependencies
        private Text cooldownText;
        #endregion

        public RTPPlayerAction AssociatedAction { set => associatedAction = value; }
        public Text CooldownText { set => cooldownText = value; }

        public static CooldownFeedLineType Instanciate(Transform parent, RTPPlayerAction associatedAction)
        {
            var cooldownFeedLine = MonoBehaviour.Instantiate(PrefabContainer.Instance.CooldownFeedLineType, parent);
            cooldownFeedLine.AssociatedAction = associatedAction;

            var iconImage = cooldownFeedLine.GetComponentInChildren<Image>();
            iconImage.sprite = SelectionWheelNodeConfiguration.selectionWheelNodeConfiguration[associatedAction.ActionWheelNodeConfigurationId].ContextActionWheelIcon;

            #region Internal Dependencies
            cooldownFeedLine.CooldownText = cooldownFeedLine.GetComponentInChildren<Text>();
            #endregion

            return cooldownFeedLine;
        }

        public void Tick()
        {
            cooldownText.text = TimeSpan.FromSeconds(Mathf.Max(associatedAction.GetCooldownRemainingTime(), 0f)).ToString(SelectionWheelNode.COOLDWON_TIMER_PATTERN);
        }

        #region Logical Condition
        public bool AreActionEquals(SelectionWheelNodeConfigurationId SelectionWheelNodeConfigurationId)
        {
            return SelectionWheelNodeConfigurationId == associatedAction.ActionWheelNodeConfigurationId;
        }
        #endregion

    }

}
