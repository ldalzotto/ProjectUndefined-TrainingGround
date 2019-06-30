using CoreGame;
using GameConfigurationID;
using System;
using UnityEngine;

namespace AdventureGame
{

    [System.Serializable]
    public abstract class AContextAction : SequencedAction
    {
        [SerializeField]
        protected SelectionWheelNodeConfigurationId contextActionWheelNodeConfigurationId;

        public AContextAction(SequencedAction nextAction) : base(nextAction)
        {
        }

        public SelectionWheelNodeConfigurationId ContextActionWheelNodeConfigurationId { get => contextActionWheelNodeConfigurationId; set => contextActionWheelNodeConfigurationId = value; }

        #region Logical Conditions
        public bool IsTalkAction()
        {
            return GetType() == typeof(TalkAction);
        }
        #endregion
    }

    [System.Serializable]
    public abstract class AContextActionInput : SequencedActionInput
    {

    }
}
