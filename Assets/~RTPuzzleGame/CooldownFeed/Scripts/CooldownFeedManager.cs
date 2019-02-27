using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class CooldownFeedManager : MonoBehaviour
    {
        private const string VERTICAL_FEED_OBJECT_NAME = "VerticalFeed";

        private CooldownFeedLineManager CooldownFeedLineManager;

        #region Internal dependencies
        private GameObject verticalFeedObject;
        #endregion

        public void Init()
        {
            verticalFeedObject = gameObject.FindChildObjectRecursively(VERTICAL_FEED_OBJECT_NAME);
            CooldownFeedLineManager = new CooldownFeedLineManager();
        }

        public void Tick(float d)
        {
            CooldownFeedLineManager.Tick();
        }

        #region External Events
        internal void OnRTPPlayerActionStop(RTPPlayerAction playerAction)
        {
            if (playerAction.IsOnCoolDown())
            {
                OnCooldownFeedLineAdd(playerAction);
            }
        }
        public void OnCooldownEnded(SelectionWheelNodeConfigurationId SelectionWheelNodeConfigurationId)
        {
            CooldownFeedLineManager.OnCooldownEnded(SelectionWheelNodeConfigurationId);
        }
        #endregion

        #region Internal Events
        private void OnCooldownFeedLineAdd(RTPPlayerAction playerAction)
        {
            CooldownFeedLineManager.OnCooldownFeedLineAdd(playerAction, verticalFeedObject.transform);
        }
        #endregion
    }

    class CooldownFeedLineManager
    {

        private List<CooldownFeedLineType> cooldownFeedLines = new List<CooldownFeedLineType>();

        public void OnCooldownFeedLineAdd(RTPPlayerAction playerAction, Transform verticalFeedTransform)
        {
            cooldownFeedLines.Add(CooldownFeedLineType.Instanciate(verticalFeedTransform, playerAction));
        }

        public void Tick()
        {
            foreach(var cooldownFeedLine in cooldownFeedLines)
            {
                cooldownFeedLine.Tick();
            }
        }

        public void OnCooldownEnded(SelectionWheelNodeConfigurationId SelectionWheelNodeConfigurationId)
        {
            CooldownFeedLineType matchedCooldownFeedLine = null;
            foreach(var cooldownFeedLine in cooldownFeedLines)
            {
                if (cooldownFeedLine.AreActionEquals(SelectionWheelNodeConfigurationId))
                {
                    matchedCooldownFeedLine = cooldownFeedLine;
                    break;
                }
            }
            if (matchedCooldownFeedLine != null)
            {
                cooldownFeedLines.Remove(matchedCooldownFeedLine);
                MonoBehaviour.Destroy(matchedCooldownFeedLine.gameObject);
            }
        }

    }
}
