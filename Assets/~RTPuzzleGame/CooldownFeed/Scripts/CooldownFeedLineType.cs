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
        private CooldownFeedLineAnimationManager CooldownFeedLineAnimationManager;
        private AnimatedLayoutCell animatedLayoutCell;
        #endregion

        public RTPPlayerAction AssociatedAction { set => associatedAction = value; }
        public Text CooldownText { set => cooldownText = value; }
        public AnimatedLayoutCell AnimatedLayoutCell { get => animatedLayoutCell; set => animatedLayoutCell = value; }

        public static CooldownFeedLineType Instanciate(Transform parent, RTPPlayerAction associatedAction)
        {
            var cooldownFeedLine = MonoBehaviour.Instantiate(PrefabContainer.Instance.CooldownFeedLineType, parent);
            cooldownFeedLine.AssociatedAction = associatedAction;

            var iconImage = cooldownFeedLine.GetComponentInChildren<Image>();
            iconImage.sprite = SelectionWheelNodeConfiguration.selectionWheelNodeConfiguration[associatedAction.GetSelectionWheelConfigurationId()].ContextActionWheelIcon;

            cooldownFeedLine.CooldownFeedLineAnimationManager = new CooldownFeedLineAnimationManager((RectTransform)cooldownFeedLine.transform);

            #region Internal Dependencies
            cooldownFeedLine.CooldownText = cooldownFeedLine.GetComponentInChildren<Text>();
            cooldownFeedLine.animatedLayoutCell = cooldownFeedLine.GetComponent<AnimatedLayoutCell>();
            #endregion

            return cooldownFeedLine;
        }

        #region External Events
        public void OnLineAdded()
        {
            CooldownFeedLineAnimationManager.OnLineAdded();
        }
        public void OnLayoutForceUpdated()
        {
            CooldownFeedLineAnimationManager.OnLayoutForceUpdated();
        }
        #endregion

        public void Tick(float d)
        {
            cooldownText.text = TimeSpan.FromSeconds(Mathf.Max(associatedAction.GetCooldownRemainingTime(), 0f)).ToString(SelectionWheelNode.COOLDWON_TIMER_PATTERN);
        }
        public bool TickAnimation(float d, float slideSpeed)
        {
            return CooldownFeedLineAnimationManager.Tick(d, slideSpeed);
        }

        #region Logical Condition
        public bool AreActionEquals(RTPPlayerAction comparedAction)
        {
            return comparedAction == associatedAction;
        }
        public bool IsMoving()
        {
            return CooldownFeedLineAnimationManager.IsMoving;
        }

        #endregion

    }

    class CooldownFeedLineAnimationManager
    {

        private RectTransform lineTransform;

        public CooldownFeedLineAnimationManager(RectTransform lineTransform)
        {
            this.lineTransform = lineTransform;
        }

        private Vector3 oldLinePosition;

        private float slideSpeed;
        private Vector3 targetPosition;
        private bool isMoving;

        public bool IsMoving { get => isMoving; }

        public bool Tick(float d, float slideSpeed)
        {
            if (isMoving)
            {
                lineTransform.position = Vector3.Lerp(lineTransform.position, targetPosition, slideSpeed * d);
                if (Vector3.Distance(lineTransform.position, targetPosition) < 1f)
                {
                    isMoving = false;
                    lineTransform.position = targetPosition;
                }
            }
            oldLinePosition = lineTransform.position;
            return isMoving;
        }

        internal void OnLineAdded()
        {
            isMoving = true;
            this.targetPosition = this.lineTransform.position;
            this.lineTransform.position = this.targetPosition + new Vector3(-200, 0, 0);
        }

        internal void OnLayoutForceUpdated()
        {
            if (!isMoving)
            {
                this.lineTransform.position = oldLinePosition;
            }
        }

    }

}
