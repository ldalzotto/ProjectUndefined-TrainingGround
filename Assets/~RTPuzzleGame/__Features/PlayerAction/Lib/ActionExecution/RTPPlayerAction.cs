using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public abstract class RTPPlayerAction
    {
        public PlayerActionType PlayerActionType;

        public abstract bool FinishedCondition();
        public abstract void Tick(float d);
        public abstract void LateTick(float d);
        public abstract void GUITick();
        public abstract void GizmoTick();

        protected CorePlayerActionDefinition CorePlayerActionDefinition;
        private SelectionWheelNodeConfigurationData SelectionWheelNodeConfigurationData;

        private float onCooldownTimeElapsed;
        //-1 is infinite
        private int remainingExecutionAmout;

        private CooldownEventTrackerManager CooldownEventTrackerManager;


        protected RTPPlayerAction(CorePlayerActionDefinition CorePlayerActionDefinition)
        {
            this.PlayerActionType = CorePlayerActionDefinition.PlayerActionType;
            var SelectionWheelNodeConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.CoreConfiguration.SelectionWheelNodeConfiguration;
            this.SelectionWheelNodeConfigurationData = SelectionWheelNodeConfiguration.ConfigurationInherentData[CorePlayerActionDefinition.ActionWheelNodeConfigurationId];

            this.CorePlayerActionDefinition = CorePlayerActionDefinition;

            //on init, it is available
            this.onCooldownTimeElapsed = this.CorePlayerActionDefinition.CoolDownTime * 2;
            this.remainingExecutionAmout = this.CorePlayerActionDefinition.ExecutionAmount;
        }

        public virtual void FirstExecution()
        {
            var playerActionEventManager = PuzzleGameSingletonInstances.PlayerActionEventManager;
            this.CooldownEventTrackerManager = new CooldownEventTrackerManager(playerActionEventManager);
        }

        public void CoolDownTick(float d)
        {
            onCooldownTimeElapsed += d;
            if (!IsOnCoolDown())
            {
                this.CooldownEventTrackerManager.Tick(this);
            }
        }

        protected void PlayerActionConsumed()
        {
            onCooldownTimeElapsed = 0f;
            this.CooldownEventTrackerManager.ResetCoolDown();
            if (this.remainingExecutionAmout > 0)
            {
                this.remainingExecutionAmout -= 1;
            }
        }

        public void IncreaseActionRemainingExecutionAmount(int deltaIncrease)
        {
            this.remainingExecutionAmout += deltaIncrease;
        }

        #region Logical Conditions
        public bool IsOnCoolDown()
        {
            return onCooldownTimeElapsed < this.CorePlayerActionDefinition.CoolDownTime;
        }
        public bool CanBeExecuted()
        {
            return !this.IsOnCoolDown() && this.HasStillSomeExecutionAmount();
        }
        public bool HasStillSomeExecutionAmount() { return this.remainingExecutionAmout != 0; }
        #endregion

        #region Data Retrieval
        public float GetCooldownRemainingTime()
        {
            return this.CorePlayerActionDefinition.CoolDownTime - onCooldownTimeElapsed;
        }
        public SelectionWheelNodeConfigurationId GetSelectionWheelConfigurationId()
        {
            return this.CorePlayerActionDefinition.ActionWheelNodeConfigurationId;
        }
        public string GetDescriptionText()
        {
            return this.SelectionWheelNodeConfigurationData.DescriptionText;
        }
        public Sprite GetNodeIcon() { return this.SelectionWheelNodeConfigurationData.WheelNodeIcon; }

        public int RemainingExecutionAmout { get => remainingExecutionAmout; }
        #endregion
    }

    #region Cooldown Tracking
    class CooldownEventTrackerManager
    {
        private PlayerActionEventManager PlayerActionEventManager;

        public CooldownEventTrackerManager(PlayerActionEventManager playerActionEventManager)
        {
            PlayerActionEventManager = playerActionEventManager;
            this.endOfCooldownEventEmitted = false;
        }

        private bool endOfCooldownEventEmitted;

        public void Tick(RTPPlayerAction involvedAction)
        {
            if (!this.endOfCooldownEventEmitted)
            {
                this.endOfCooldownEventEmitted = true;
                PlayerActionEventManager.OnCooldownEnded(involvedAction);
            }
        }
        public void ResetCoolDown()
        {
            this.endOfCooldownEventEmitted = false;
        }

    }
    #endregion
}

