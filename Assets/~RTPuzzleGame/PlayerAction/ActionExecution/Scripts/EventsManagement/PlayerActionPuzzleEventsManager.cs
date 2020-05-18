﻿using CoreGame;
using GameConfigurationID;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class PlayerActionPuzzleEventsManager : MonoBehaviour, SelectableObjectSelectionManagerEventListener<SelectableObject>
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

        public void OnSelectableObjectSelected(SelectableObject SelectableObject)
        {
            this.PlayerActionManager.OnSelectableObjectSelected(SelectableObject);
        }
        public void OnSelectableObjectDeSelected(SelectableObject SelectableObject)
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
