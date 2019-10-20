using System;
using System.Collections.Generic;
using CoreGame;
using RTPuzzle;
using SelectableObjects_Interfaces;

namespace PlayerActions
{
    public class PlayerActionEventManager : GameSingleton<PlayerActionEventManager>
    {
        private PlayerActionManager PlayerActionManager = PlayerActionManager.Get();
        private PlayerActionWheelManager PlayerActionWheelManager = PlayerActionWheelManager.Get();

        public void Init()
        {
            #region Event Register

            SelectableObjectEventsManager.Get().RegisterOnSelectableObjectSelectedEventAction(this.OnSelectableObjectSelected);
            SelectableObjectEventsManager.Get().RegisterOnSelectableObjectNoMoreSelectedEventAction(this.OnSelectableObjectDeSelected);

            #endregion
        }

        public void IncreaseOrAddActionsRemainingExecutionAmount(RTPPlayerAction RTPPlayerAction, int deltaRemaining)
        {
            this.PlayerActionManager.IncreaseOrAddActionsRemainingExecutionAmount(RTPPlayerAction, deltaRemaining);
        }


        public void ExecuteAction(RTPPlayerAction rTPPlayerAction)
        {
            this.PlayerActionManager.ExecuteAction(rTPPlayerAction);
            this.PlayerActionWheelManager.PlayerActionWheelSleep(false);
        }

        public void AwakePlayerActionSelectionWheel()
        {
            this.PlayerActionWheelManager.PlayerActionWheelAwake(this.PlayerActionManager.GetCurrentAvailablePlayerActions());
        }

        public void SleepPlayerActionSelectionWheel(bool destroyImmediate)
        {
            this.PlayerActionWheelManager.PlayerActionWheelSleep(destroyImmediate);
        }

        public void AddActionsToAvailable(List<RTPPlayerAction> addedActions)
        {
            this.PlayerActionManager.AddActionsToAvailable(addedActions);
            this.PlayerActionWheelManager.PlayerActionWheelRefresh(this.PlayerActionManager.GetCurrentAvailablePlayerActions());
        }

        public void RemoveActionsToAvailable(List<RTPPlayerAction> removedActions)
        {
            this.PlayerActionManager.RemoveActionsToAvailable(removedActions);
            this.PlayerActionWheelManager.PlayerActionWheelRefresh(this.PlayerActionManager.GetCurrentAvailablePlayerActions());
        }

        private void OnSelectableObjectSelected(ISelectableObjectSystem SelectableObject)
        {
            this.PlayerActionManager.AddActionToAvailable(SelectableObject.AssociatedPlayerAction as RTPPlayerAction);
            this.PlayerActionWheelManager.PlayerActionWheelRefresh(this.PlayerActionManager.GetCurrentAvailablePlayerActions());
        }

        private void OnSelectableObjectDeSelected(ISelectableObjectSystem SelectableObject)
        {
            this.PlayerActionManager.RemoveActionToAvailable(SelectableObject.AssociatedPlayerAction as RTPPlayerAction);
            this.PlayerActionWheelManager.PlayerActionWheelRefresh(this.PlayerActionManager.GetCurrentAvailablePlayerActions());
        }
    }
}