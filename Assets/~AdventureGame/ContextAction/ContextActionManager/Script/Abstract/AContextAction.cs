using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    [System.Serializable]
    public abstract class AContextAction : SequencedAction
    {
        [SerializeField]
        [CustomEnum(ConfigurationType = typeof(SelectionWheelNodeConfiguration))]
        public SelectionWheelNodeConfigurationId contextActionWheelNodeConfigurationId;

        public AContextAction(List<SequencedAction> nextActions) : base(nextActions)
        {
            var selectionWheelNodeConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.SelectionWheelNodeConfiguration;
        }

        #region Logical Conditions
        public bool IsTalkAction()
        {
            return GetType() == typeof(TalkAction);
        }
        #endregion

        #region Data Retrieval
        public Sprite GetIconSprite() { return CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.SelectionWheelNodeConfiguration.ConfigurationInherentData[this.contextActionWheelNodeConfigurationId].WheelNodeIcon; }
        public string GetActionDescription() { return CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.SelectionWheelNodeConfiguration.ConfigurationInherentData[this.contextActionWheelNodeConfigurationId].DescriptionText; }
        #endregion
    }

    [System.Serializable]
    public abstract class AContextActionInput : SequencedActionInput
    {

    }
}
