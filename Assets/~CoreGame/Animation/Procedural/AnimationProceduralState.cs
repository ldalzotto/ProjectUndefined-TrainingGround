namespace CoreGame
{
    public abstract class AnimationProceduralState
    {
        public delegate void OnJustDisabled();
        public delegate void OnJustEnabled();

       public event OnJustDisabled onJustDisabled;
        public event OnJustEnabled onJustEnabled;

        protected float currentElapsedTimeFromActive;
        private bool isEnabled;

        public bool IsEnabled { get => isEnabled; }
        public void SetIsEnabled(bool value)
        {
            if (value && !this.isEnabled)
            {
                this.currentElapsedTimeFromActive = 0f;
                if (this.onJustEnabled != null)
                {
                    this.onJustEnabled.Invoke();
                }
            }
            else if (!value && this.isEnabled)
            {
                if (this.onJustDisabled != null)
                {
                    this.onJustDisabled.Invoke();
                }
            }
            this.isEnabled = value;
        }

        public void LateTick(float d)
        {
            if (this.IsEnabled)
            {
                this.currentElapsedTimeFromActive += d;
                this.LateTickImpl(d);
            }
        }

        protected abstract void LateTickImpl(float d);


    }
}
