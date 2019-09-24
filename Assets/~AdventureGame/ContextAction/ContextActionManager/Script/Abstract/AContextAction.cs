using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    [System.Serializable]
    public abstract class AContextAction : SequencedAction
    {
        [SerializeField]
        [CustomEnum(ConfigurationType = typeof(SelectionWheelNodeConfiguration))]
        public SelectionWheelNodeConfigurationId ContextActionWheelNodeConfigurationId;

        private SelectionWheelNodeConfigurationData selectionWheelNodeConfigurationData;

        public AContextAction(List<SequencedAction> nextActions, SelectionWheelNodeConfigurationId SelectionWheelNodeConfigurationId) : base(nextActions)
        {
            var selectionWheelNodeConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.SelectionWheelNodeConfiguration;
            this.ContextActionWheelNodeConfigurationId = SelectionWheelNodeConfigurationId;
            this.selectionWheelNodeConfigurationData = selectionWheelNodeConfiguration.ConfigurationInherentData[this.ContextActionWheelNodeConfigurationId];
        }

        #region Logical Conditions
        public bool IsTalkAction()
        {
            return GetType() == typeof(TalkAction);
        }
        #endregion

        #region Data Retrieval
        public Sprite GetIconSprite() { return this.selectionWheelNodeConfigurationData.WheelNodeIcon; }
        public string GetActionDescription() { return this.selectionWheelNodeConfigurationData.DescriptionText; }
        #endregion
    }

    [System.Serializable]
    public abstract class AContextActionInput : SequencedActionInput
    {

    }
}
