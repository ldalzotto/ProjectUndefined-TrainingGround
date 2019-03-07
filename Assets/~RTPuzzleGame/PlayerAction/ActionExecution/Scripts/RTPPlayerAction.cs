using UnityEngine;

namespace RTPuzzle
{
    public abstract class RTPPlayerAction
    {
      //  public abstract SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId { get; }

        public abstract bool FinishedCondition();
        public abstract void Tick(float d);
        public abstract void GUITick();
        public abstract void GizmoTick();

        protected PlayerActionInherentData playerActionInherentData;
      //  private float cooldownTime;
        private float onCooldownTimeElapsed;

        private CooldownEventTrackerManager CooldownEventTrackerManager;

        protected RTPPlayerAction(PlayerActionInherentData playerActionInherentData)
        {
            this.playerActionInherentData = playerActionInherentData;
           // this.cooldownTime = cooldownTime;
            //on init, it it available
            this.onCooldownTimeElapsed = this.playerActionInherentData.CoolDownTime * 2;
        }

        public virtual void FirstExecution()
        {
            var playerActionEventManager = GameObject.FindObjectOfType<PlayerActionEventManager>();
            this.CooldownEventTrackerManager = new CooldownEventTrackerManager(playerActionEventManager);
        }

        public void CoolDownTick(float d)
        {
            onCooldownTimeElapsed += d;
            if( !IsOnCoolDown())
            {
                this.CooldownEventTrackerManager.Tick(this);
            }
        }

        protected void ResetCoolDown()
        {
            onCooldownTimeElapsed = 0f;
            this.CooldownEventTrackerManager.ResetCoolDown();
        }

        #region Logical Conditions
        public bool IsOnCoolDown()
        {
            return onCooldownTimeElapsed < this.playerActionInherentData.CoolDownTime;
        }
        #endregion

        #region Data Retrieval
        public float GetCooldownRemainingTime()
        {
            return this.playerActionInherentData.CoolDownTime - onCooldownTimeElapsed;
        }
        public SelectionWheelNodeConfigurationId GetSelectionWheelConfigurationId()
        {
            return this.playerActionInherentData.ActionWheelNodeConfigurationId;
        }
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
                Debug.Log(PlayerActionEventManager);
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

