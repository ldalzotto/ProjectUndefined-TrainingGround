using CoreGame;
using GameConfigurationID;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class PlayerActionPuzzleEventsManager : MonoBehaviour, SelectableObjectSelectionManagerEventListener<ISelectableModule>
    {

        #region External Dependencies
        private PlayerActionManager PlayerActionManager;
        #endregion

        public void Init()
        {
            this.PlayerActionManager = GameObject.FindObjectOfType<PlayerActionManager>();
        }

        #region External Puzzle Events
        public void OnThrowProjectileCursorOnProjectileRange()
        {
            RTPActionValidation((RTPPlayerAction currentAction) =>
            {
                if (currentAction.GetType() == typeof(LaunchProjectileAction))
                {
                    ((LaunchProjectileAction)currentAction).OnThrowProjectileCursorOnProjectileRange();
                }
            }
            );
        }
        public void OnThrowProjectileCursorOutOfProjectileRange()
        {
            RTPActionValidation((RTPPlayerAction currentAction) =>
            {
                if (currentAction.GetType() == typeof(LaunchProjectileAction))
                {
                    ((LaunchProjectileAction)currentAction).OnThrowProjectileCursorOutOfProjectileRange();
                }
            }
          );
        }

        public void OnSelectableObjectSelected(ISelectableModule SelectableObject)
        {
            this.PlayerActionManager.OnSelectableObjectSelected(SelectableObject);
        }
        public void OnSelectableObjectDeSelected(ISelectableModule SelectableObject)
        {
            this.PlayerActionManager.OnSelectableObjectDeSelected(SelectableObject);
        }
        
        #endregion

        private void RTPActionValidation(Action<RTPPlayerAction> ifValidatedAction)
        {
            var runningAction = this.PlayerActionManager.GetCurrentRunningAction();
            if (runningAction != null)
            {
                ifValidatedAction.Invoke(runningAction);
            }
        }

    }

}
