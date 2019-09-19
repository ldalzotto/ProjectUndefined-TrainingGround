using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface ICooldownFeedManagerEvent
    {
        void OnRTPPlayerActionStop(RTPPlayerAction playerAction);
        void OnCooldownEnded(RTPPlayerAction involvedAction);
    }

    public class CooldownFeedManager : MonoBehaviour, ICooldownFeedManagerEvent
    {
        private CooldownFeedLineManager CooldownFeedLineManager;
        private AnimatedLayout AnimatedLayout;

        public void Init()
        {
            AnimatedLayout = GetComponent<AnimatedLayout>();
            AnimatedLayout.Init();

            CooldownFeedLineManager = new CooldownFeedLineManager();
        }

        public void Tick(float d)
        {
            AnimatedLayout.Tick(d);
            CooldownFeedLineManager.Tick(d);
        }

        #region External Events
        public void OnRTPPlayerActionStop(RTPPlayerAction playerAction)
        {
            if (playerAction.IsOnCoolDown())
            {
                OnCooldownFeedLineAdd(playerAction);
            }
        }
        public void OnCooldownEnded(RTPPlayerAction involvedAction)
        {
            var deletedLine = CooldownFeedLineManager.OnCooldownEnded(involvedAction);
            if(deletedLine != null)
            {
                AnimatedLayout.DeleteLayoutElement(deletedLine.AnimatedLayoutCell);
            }
        }
        #endregion

        #region Internal Events
        private void OnCooldownFeedLineAdd(RTPPlayerAction playerAction)
        {
            var addedLine = CooldownFeedLineManager.OnCooldownFeedLineAdd(playerAction, transform);
            AnimatedLayout.AddLayoutElement(addedLine.AnimatedLayoutCell, 0);
        }
        #endregion
    }

    class CooldownFeedLineManager
    {

        private List<CooldownFeedLineType> cooldownFeedLines = new List<CooldownFeedLineType>();

        public List<CooldownFeedLineType> CooldownFeedLines { get => cooldownFeedLines; }

        public CooldownFeedLineType OnCooldownFeedLineAdd(RTPPlayerAction playerAction, Transform verticalFeedTransform)
        {
            var addedLine = CooldownFeedLineType.Instanciate(verticalFeedTransform, playerAction);
            cooldownFeedLines.Add(addedLine);
            return addedLine;
        }

        public void Tick(float d)
        {
            foreach (var cooldownFeedLine in cooldownFeedLines)
            {
                cooldownFeedLine.Tick(d);
            }
        }

        public CooldownFeedLineType OnCooldownEnded(RTPPlayerAction involvedAction)
        {
            CooldownFeedLineType matchedCooldownFeedLine = null;
            foreach (var cooldownFeedLine in cooldownFeedLines)
            {
                if (cooldownFeedLine.AreActionEquals(involvedAction))
                {
                    matchedCooldownFeedLine = cooldownFeedLine;
                    break;
                }
            }
            if (matchedCooldownFeedLine != null)
            {
                cooldownFeedLines.Remove(matchedCooldownFeedLine);
            }
            return matchedCooldownFeedLine;
        }

    }
}
