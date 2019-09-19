using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class PlayerActionEventManager : MonoBehaviour
    {
        #region External Dependencies
        private PlayerActionManager PlayerActionManager;
        private ICooldownFeedManagerEvent ICooldownFeedManagerEvent;
        #endregion

        public void Init()
        {
            PlayerActionManager = PuzzleGameSingletonInstances.PlayerActionManager;
            ICooldownFeedManagerEvent = PuzzleGameSingletonInstances.CooldownFeedManager;
        }
        
        public void OnRTPPlayerActionStop(RTPPlayerAction stoppedAction)
        {
            PlayerActionManager.StopAction();
            ICooldownFeedManagerEvent.OnRTPPlayerActionStop(stoppedAction);
        }
        
        public void OnCooldownEnded(RTPPlayerAction involvedAction)
        {
            ICooldownFeedManagerEvent.OnCooldownEnded(involvedAction);
        }
        
    }

}
