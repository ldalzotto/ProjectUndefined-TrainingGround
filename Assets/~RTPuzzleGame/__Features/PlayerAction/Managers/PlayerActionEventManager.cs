using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class PlayerActionEventManager : MonoBehaviour
    {
        #region External Dependencies
        private ICooldownFeedManagerEvent ICooldownFeedManagerEvent;
        #endregion

        public void Init()
        {
            ICooldownFeedManagerEvent = PuzzleGameSingletonInstances.CooldownFeedManager;
        }
        
        public void OnRTPPlayerActionStop(RTPPlayerAction stoppedAction)
        {
            PlayerActionManager.Get().StopAction();
            ICooldownFeedManagerEvent.OnRTPPlayerActionStop(stoppedAction);
        }
        
        public void OnCooldownEnded(RTPPlayerAction involvedAction)
        {
            ICooldownFeedManagerEvent.OnCooldownEnded(involvedAction);
        }
        
    }

}
