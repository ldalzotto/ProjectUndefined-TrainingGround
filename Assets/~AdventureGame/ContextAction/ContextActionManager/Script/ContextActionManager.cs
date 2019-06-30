using CoreGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class ContextActionManager : SequencedActionManager
    {
        #region External Dependencies
        private ContextActionEventManager ContextActionEventManager;
        #endregion

        protected override Action<SequencedAction> OnActionAdd => ContextActionEventManager.OnContextActionAdd;

        protected override Action<SequencedAction, SequencedActionInput> OnActionFinished => ContextActionEventManager.OnContextActionFinished;

        private void Start()
        {
            ContextActionEventManager = GameObject.FindObjectOfType<ContextActionEventManager>();
        }
    }
}