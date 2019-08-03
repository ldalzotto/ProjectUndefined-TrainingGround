using GameConfigurationID;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class PlayerActionPuzzleEventsManager : MonoBehaviour
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
        public void OnActionInteractableEnter(ActionInteractableObjectModule actionInteractableObjectModule)
        {
            this.PlayerActionManager.AddActionToAvailable(PlayerActionId.NONE, actionInteractableObjectModule.AssociatedPlayerAction);
        }
        public void OnActionInteractableExit(ActionInteractableObjectModule actionInteractableObjectModule)
        {
            this.PlayerActionManager.RemoveActionToAvailable(PlayerActionId.NONE, actionInteractableObjectModule.AssociatedPlayerAction);
        }
        public void OnGrabObjectEnter(GrabObjectModule grabObjectModule)
        {
            this.PlayerActionManager.AddActionToAvailable(PlayerActionId.NONE, grabObjectModule.GrabObjectAction);
        }
        public void OnGrabObjectExit(GrabObjectModule grabObjectModule)
        {
            this.PlayerActionManager.RemoveActionToAvailable(PlayerActionId.NONE, grabObjectModule.GrabObjectAction);
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
