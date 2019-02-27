namespace RTPuzzle
{
    public abstract class RTPPlayerAction
    {
        public abstract SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId { get; }

        public abstract bool FinishedCondition();
        public abstract void FirstExecution();
        public abstract void Tick(float d);
        public abstract void GUITick();
        public abstract void GizmoTick();

        private float cooldownTime;
        private float onCooldownTimeElapsed;

        protected RTPPlayerAction(float cooldownTime)
        {
            this.cooldownTime = cooldownTime;
            //on init, it it available
            this.onCooldownTimeElapsed = this.cooldownTime * 2;
        }

        public void CoolDownTick(float d)
        {
            onCooldownTimeElapsed += d;
        }

        protected void ResetCoolDown()
        {
            onCooldownTimeElapsed = 0f;
        }

        #region Logical Conditions
        public bool IsOnCoolDown()
        {
            return onCooldownTimeElapsed < cooldownTime;
        }
        #endregion

        #region Data Retrieval
        public float GetCooldownRemainingTime()
        {
            return cooldownTime - onCooldownTimeElapsed;
        }
        #endregion
    }
}

